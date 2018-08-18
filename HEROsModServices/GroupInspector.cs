﻿using HEROsMod.UIKit;
using HEROsMod.UIKit.UIComponents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace HEROsMod.HEROsModServices
{
	internal class GroupInspector : HEROsModService
	{
		private GroupManagementWindow groupWindow;

		public GroupInspector(UIHotbar hotbar)
		{
			IsInHotbar = true;
			HotbarParent = hotbar;
			MultiplayerOnly = true;
			_name = "Group Inspector";
			_hotbarIcon = new UIImage(HEROsMod.instance.GetTexture("Images/manageGroups"));
			HotbarIcon.OnLeftClick += HotbarIcon_onLeftClick;
			HotbarIcon.Tooltip = HEROsMod.HeroText("OpenGroupManagement");
			HEROsModNetwork.LoginService.GroupChanged += LoginService_GroupChanged;
		}

		private void LoginService_GroupChanged(object sender, EventArgs e)
		{
			if (groupWindow != null)
			{
				groupWindow.RefreshGroupList();
			}
		}

		private void HotbarIcon_onLeftClick(object sender, EventArgs e)
		{
			if (groupWindow == null)
			{
				groupWindow = new GroupManagementWindow();
				groupWindow.Closed += GroupWindow_Closed;
				AddUIView(groupWindow);
			}
			else
			{
				groupWindow.Close();
			}
		}

		private void GroupWindow_Closed(object sender, EventArgs e) => groupWindow = null;

		public override void MyGroupUpdated()
		{
			HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.IsAdmin;
			if (!HasPermissionToUse)
			{
				if (groupWindow != null)
				{
					groupWindow.Close();
				}
			}
			//base.MyGroupUpdated();
		}

		public override void Destroy()
		{
			HEROsModNetwork.LoginService.GroupChanged -= LoginService_GroupChanged;
			base.Destroy();
		}
	}

	internal class GroupManagementWindow : UIWindow
	{
		private static float spacing = 16f;

		//Group group;
		private UIDropdown dropdown = new UIDropdown();

		private UIScrollView checkboxContainer = new UIScrollView();

		public event EventHandler Closed;

		public GroupManagementWindow()
		{
			UILabel title = new UILabel(HEROsMod.HeroText("GroupManagement"));
			UIButton bApply = new UIButton(HEROsMod.HeroText("Apply"));
			UIButton bDelete = new UIButton(HEROsMod.HeroText("DeleteGroup"));
			UIButton bNew = new UIButton(HEROsMod.HeroText("NewGroup"));
			UILabel label = new UILabel(HEROsMod.HeroText("Groups") + ":");
			UIImage bClose = new UIImage(closeTexture);
			dropdown.SelectedChanged += Dropdown_selectedChanged;
			Anchor = AnchorPosition.Center;
			CanMove = true;

			Width = 700;
			title.Scale = .6f;
			title.X = spacing;
			title.Y = spacing;
			title.OverridesMouse = false;
			bClose.X = Width - bClose.Width - spacing;
			bClose.Y = spacing;
			label.X = spacing;
			label.Y = title.Y + title.Height;
			label.Scale = .5f;
			dropdown.X = label.X + label.Width + 4;
			dropdown.Y = label.Y;
			dropdown.Width = 200;
			checkboxContainer.X = spacing;
			checkboxContainer.Y = dropdown.Y + dropdown.Height + spacing;
			checkboxContainer.Width = Width - spacing * 2;
			checkboxContainer.Height = 150;
			AddChild(checkboxContainer);
			AddChild(label);
			AddChild(title);

			RefreshGroupList();

			bApply.Anchor = AnchorPosition.TopRight;
			bApply.X = Width - spacing;
			bApply.Y = checkboxContainer.Y + checkboxContainer.Height + spacing;
			bNew.X = spacing;
			bNew.Y = bApply.Y;
			bNew.AutoSize = false;
			bNew.Width = 100;
			bDelete.X = bNew.X + bNew.Width + 8;
			bDelete.Y = bNew.Y;
			bDelete.AutoSize = false;
			bDelete.Width = 100;

			bApply.OnLeftClick += BApply_onLeftClick;
			bClose.OnLeftClick += BClose_onLeftClick;
			bNew.OnLeftClick += BNew_onLeftClick;
			bDelete.OnLeftClick += BDelete_onLeftClick;

			AddChild(bApply);
			AddChild(bClose);
			AddChild(bNew);
			AddChild(bDelete);
			AddChild(dropdown);

			Height = bApply.Position.Y + bApply.Height + spacing;
			CenterToParent();
		}

		private void BDelete_onLeftClick(object sender, EventArgs e)
		{
			UIMessageBox mb = new UIMessageBox(string.Format(HEROsMod.HeroText("AreYouSureDeleteGroup"), dropdown.Text), UIMessageBoxType.YesNo, true);
			mb.YesClicked += Mb_yesClicked;
			Parent.AddChild(mb);
		}

		private void Mb_yesClicked(object sender, EventArgs e) => HEROsModNetwork.LoginService.RequestDeleteGroup(HEROsModNetwork.Network.Groups[dropdown.SelectedItem].ID);

		private void BNew_onLeftClick(object sender, EventArgs e) => Parent.AddChild(new NewGroupWindow());

		private void BClose_onLeftClick(object sender, EventArgs e) => Close();

		private void BApply_onLeftClick(object sender, EventArgs e)
		{
			HEROsModNetwork.Group group = new HEROsModNetwork.Group(dropdown.GetItem(dropdown.SelectedItem))
			{
				ID = HEROsModNetwork.Network.Groups[dropdown.SelectedItem].ID
			};
			group.ImportPermissions(ExportPermissions());
			HEROsModNetwork.LoginService.RequestSetGroupPermissions(group);
		}

		public void RefreshGroupList()
		{
			int prevNumOfGroups = dropdown.ItemCount;
			int prevSelected = dropdown.SelectedItem;
			dropdown.ClearItems();
			for (int i = 0; i < HEROsModNetwork.Network.Groups.Count; i++)
			{
				dropdown.AddItem(HEROsModNetwork.Network.Groups[i].Name);
			}
			if (dropdown.ItemCount == prevNumOfGroups)
			{
				dropdown.SelectedItem = prevSelected;
			}
			RefreshGroupPermissions();
		}

		public void RefreshGroupPermissions()
		{
			checkboxContainer.ClearContent();
			for (int i = 0; i < HEROsModNetwork.Group.PermissionList.Count; i++)
			{
				UICheckbox cb = new UICheckbox(HEROsModNetwork.Group.PermissionList[i].Description)
				{
					Selected = HEROsModNetwork.Network.Groups[dropdown.SelectedItem].HasPermission(HEROsModNetwork.Group.PermissionList[i].Key)
				};
				checkboxContainer.AddChild(cb);
				int index = i;
				cb.X = spacing + index % 2 * (Width / 2);
				cb.Y = index / 2 * (cb.Height) + spacing;
			}
			if (checkboxContainer.ChildCount > 0)
			{
				checkboxContainer.ContentHeight = checkboxContainer.Children.Last().Y + checkboxContainer.Children.Last().Height + spacing;
			}
		}

		private byte[] ExportPermissions()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					int numberOfPermissions = 0;
					List<string> permissions = new List<string>();
					for (int i = 1; i < checkboxContainer.ChildCount; i++)
					{
						UICheckbox cb = (UICheckbox)checkboxContainer.Children[i];
						if (cb.Selected)
						{
							numberOfPermissions++;
							permissions.Add(HEROsModNetwork.Group.PermissionList[i - 1].Key);
						}
					}
					writer.Write(numberOfPermissions);
					foreach (string p in permissions)
					{
						writer.Write(p);
					}
					writer.Close();
					memoryStream.Close();
					return memoryStream.ToArray();
				}
			}
		}

		private void Dropdown_selectedChanged(object sender, EventArgs e) => RefreshGroupPermissions();

		public override void Update()
		{
			if (Main.gameMenu)
			{
				Close();
			}

			base.Update();
		}

		public void Close()
		{
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}

			Closed?.Invoke(this, EventArgs.Empty);
		}
	}

	internal class NewGroupWindow : UIWindow
	{
		private UILabel label = null;
		private UITextbox textbox = null;
		private static float spacing = 8f;

		public NewGroupWindow()
		{
			ExclusiveControl = this;

			Height = 100;
			Anchor = AnchorPosition.Center;

			label = new UILabel(HEROsMod.HeroText("GroupName") + ":");
			textbox = new UITextbox();
			UIButton bSave = new UIButton(HEROsMod.HeroText("Create"));
			UIButton bCancel = new UIButton(HEROsMod.HeroText("Cancel"));

			label.Scale = .5f;

			label.Anchor = AnchorPosition.Left;
			textbox.Anchor = AnchorPosition.Left;
			bSave.Anchor = AnchorPosition.BottomRight;
			bCancel.Anchor = AnchorPosition.BottomRight;

			float tby = textbox.Height / 2 + spacing;
			label.Position = new Vector2(spacing, tby);
			textbox.Position = new Vector2(label.Position.X + label.Width + spacing, tby);
			bCancel.Position = new Vector2(Width - spacing, Height - spacing);
			bSave.Position = new Vector2(bCancel.Position.X - bCancel.Width - spacing, bCancel.Position.Y);

			bCancel.OnLeftClick += BCancel_onLeftClick;
			bSave.OnLeftClick += BSave_onLeftClick;
			textbox.OnEnterPress += BSave_onLeftClick;

			AddChild(label);
			AddChild(textbox);
			AddChild(bSave);
			AddChild(bCancel);

			textbox.Focus();
		}

		private void BSave_onLeftClick(object sender, EventArgs e)
		{
			if (textbox.Text.Length > 0)
			{
				textbox.Unfocus();
				HEROsModNetwork.LoginService.RequestAddGroup(textbox.Text);
				Close();
			}
		}

		private void BCancel_onLeftClick(object sender, EventArgs e) => Close();

		protected new float Width => textbox.Width + label.Width + spacing * 4;

		private void Close()
		{
			ExclusiveControl = null;
			Parent.RemoveChild(this);
		}

		public override void Update()
		{
			if (Parent != null)
			{
				Position = new Vector2(Parent.Width / 2, Parent.Height / 2);
			}

			base.Update();
		}
	}
}