using Terraria;

namespace HEROsMod.UIKit.UIComponents
{
	internal class ItemCollectionView : UIScrollView
	{
		public Item[] Items
		{
			get => Items;
			set
			{
				Items = value;
				RepopulateSlots();
			}
		}

		private readonly int slotSpace = 4;
		private readonly int slotColumns = 8;
		private readonly float slotSize = Slot.backgroundTexture.Width * .85f;
		private readonly int slotRows = 4;
		private Slot[] slots = new Slot[Main.itemTexture.Length];

		public ItemCollectionView()
		{
			Width = (slotSize + slotSpace) * slotColumns + slotSpace + 20;
			Height = (slotSize + slotSpace) * slotRows + slotSpace + 20;//300;

			int numOfSlots = slotRows * slotColumns;

			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = new Slot(0);
				Slot slot = slots[i];
				int x = i % slotColumns;
				int y = i / slotColumns;
				slot.X = slotSpace + x * (slot.Width + slotSpace);
				slot.Y = slotSpace + y * (slot.Height + slotSpace);
			}
		}

		public void RepopulateSlots()
		{
			ClearContent();
			for (int i = 0; i < slots.Length; i++)
			{
				Slot slot = slots[i];
				if (i < Items.Length)
				{
					slot.Visible = true;
					slot.item = Items[i];
                    ContentHeight = slot.Y + slot.Height + Spacing;
					AddChild(slot);
				}
				else
				{
					slots[i].Visible = false;
				}
			}
		}
	}
}