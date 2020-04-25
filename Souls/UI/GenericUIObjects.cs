using System.Collections.Generic;

using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	internal class GenericUIList : UIElement
	{
		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerList : UIElement
		{
			public override bool ContainsPoint(Vector2 point)
			{
				return true;
			}

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 position = Parent.GetDimensions().Position();
				Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
				foreach (UIElement current in Elements)
				{
					Vector2 position2 = current.GetDimensions().Position();
					Vector2 dimensions2 = new Vector2(current.GetDimensions().Width, current.GetDimensions().Height);
					if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
					{
						current.Draw(spriteBatch);
					}
				}
			}
		}

		public List<UIElement> _items = new List<UIElement>();

		protected SoulIndexUIScrollbar _scrollbar;

		internal UIElement _innerList = new UIInnerList();

		private float _innerListHeight;

		public float ListPaddingX = 5f;
		public float ListPaddingY = 5f;

		public int Count => _items.Count;

		public int ListWidth = 1;

		public float ViewPosition
		{
			get { return _scrollbar.ViewPosition; }
			set { _scrollbar.ViewPosition = value; }
		}

		public GenericUIList()
		{
			_innerList.OverflowHidden = false;
			_innerList.Width.Set(0f, 1f);
			_innerList.Height.Set(0f, 1f);
			OverflowHidden = true;
			Append(_innerList);
		}

		public float GetTotalHeight()
			=> _innerListHeight;

		public void Goto(ElementSearchMethod searchMethod)
		{
			int i = 0;
			while (true)
			{
				if (i < _items.Count)
				{
					if (searchMethod(_items[i]))
					{
						break;
					}
					i++;
					continue;
				}
				return;
			}
			_scrollbar.ViewPosition = _items[i].Top.Pixels;
		}

		public virtual void Add(UIElement item)
		{
			_items.Add(item);
			_innerList.Append(item);
			UpdateOrder();
			_innerList.Recalculate();
		}

		public virtual void AddRange(IEnumerable<UIElement> items)
		{
			_items.AddRange(items);
			foreach (UIElement item in items)
			{
				_innerList.Append(item);
			}
			UpdateOrder();
			_innerList.Recalculate();
		}

		public virtual bool Remove(UIElement item)
		{
			_innerList.RemoveChild(item);
			UpdateOrder();
			return _items.Remove(item);
		}

		public virtual void Clear()
		{
			_innerList.RemoveAllChildren();
			_items.Clear();
		}

		public override void Recalculate()
		{
			base.Recalculate();
			UpdateScrollbar();
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			base.ScrollWheel(evt);
			if (_scrollbar != null)
			{
				_scrollbar.SetViewPosition(_scrollbar.ViewPosition - (evt.ScrollWheelValue / this.ListWidth));
			}
		}

		public override void RecalculateChildren()
		{
			base.RecalculateChildren();

			float currentY = 0f;
			float currentX = 0f;
			for (int i = 0; i < _items.Count; i++)
			{
				_items[i].Top.Set(currentY, 0f);
				_items[i].Left.Set(currentX, 0f);
				_items[i].Recalculate();

				if ((i + 1) % this.ListWidth == 0)
				{
					currentX = 0;
					currentY += _items[i].GetOuterDimensions().Height + ListPaddingY;
				}
				else
					currentX += _items[i].GetOuterDimensions().Width + ListPaddingX;
			}

			if (_items.Count != 0 && _items.Count % this.ListWidth != 0)
				currentY += _items[0].GetOuterDimensions().Height + ListPaddingY;

			_innerListHeight = currentY;
		}

		private void UpdateScrollbar()
		{
			if (_scrollbar != null)
			{
				_scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
			}
		}

		public void SetScrollbar(SoulIndexUIScrollbar scrollbar)
		{
			_scrollbar = scrollbar;
			UpdateScrollbar();
		}

		public void UpdateOrder()
		{
			_items.Sort(SortMethod);
			UpdateScrollbar();
		}

		public int SortMethod(UIElement item1, UIElement item2)
		{
			return item1.CompareTo(item2);
		}

		public override List<SnapPoint> GetSnapPoints()
		{
			List<SnapPoint> list = new List<SnapPoint>();
			if (GetSnapPoint(out SnapPoint item))
			{
				list.Add(item);
			}
			foreach (UIElement current in _items)
			{
				list.AddRange(current.GetSnapPoints());
			}
			return list;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_scrollbar != null)
			{
				_innerList.Top.Set(0f - _scrollbar.GetValue(), 0f);
			}
			Recalculate();
		}
	}

	internal class GenericUIScrollbar : UIElement
	{
		private float viewPosition;
		private float smoothViewPosition;

		private float viewSize = 1f;

		private float maxViewSize = 20f;

		private bool isDragging;

		private bool isHoveringOverHandle;

		private float dragYOffset;

		protected int thumbStumpSize, backgroundStumpSize;
		protected Texture2D thumb, background;

		public bool smoothScroll;

		public float ViewPosition
		{
			get { return viewPosition; }
			set { viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize); }
		}

		public GenericUIScrollbar()
		{
			Width.Set(20f, 0f);
			MaxWidth.Set(20f, 0f);
			PaddingTop = 5f;
			PaddingBottom = 5f;

			thumbStumpSize = backgroundStumpSize = 6;

			smoothScroll = false;
		}

		public void SetView(float viewSize, float maxViewSize)
		{
			viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
			viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
			this.viewSize = viewSize;
			this.maxViewSize = maxViewSize;
		}
		public void SetViewPosition(float viewPosition)
		{
			if (this.smoothScroll)
				this.smoothViewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
			else
				this.ViewPosition = viewPosition;
		}

		public override void Update(GameTime gameTime)
		{
			if (this.smoothScroll)
			{
				if (this.viewPosition < this.smoothViewPosition - .1f || this.viewPosition > this.smoothViewPosition + .1f)
				{
					this.viewPosition = MathHelper.Lerp(this.viewPosition, this.smoothViewPosition, .1f);
				}
				else
					this.viewPosition = this.smoothViewPosition;
			}
			base.Update(gameTime);
		}

		public float GetValue() => viewPosition;

		private Rectangle GetHandleRectangle()
		{
			CalculatedStyle innerDimensions = GetInnerDimensions();
			if (maxViewSize == 0f && viewSize == 0f)
			{
				viewSize = 1f;
				maxViewSize = 1f;
			}
			return new Rectangle((int)innerDimensions.X, (int)(innerDimensions.Y + innerDimensions.Height * (viewPosition / maxViewSize)) + (thumbStumpSize / 2), 20, (int)(innerDimensions.Height * (viewSize / maxViewSize)) - thumbStumpSize);
		}

		private void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, int stumpSize, Color color)
		{
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - stumpSize, dimensions.Width, stumpSize), new Rectangle(0, 0, texture.Width, stumpSize), color);
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, stumpSize, texture.Width, 4), color);
			spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, stumpSize), new Rectangle(0, texture.Height - stumpSize, texture.Width, stumpSize), color);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			CalculatedStyle innerDimensions = GetInnerDimensions();

			if (isDragging)
			{
				float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - dragYOffset;

				if (smoothScroll)
					smoothViewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
				else
					viewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
			}

			Rectangle handleRectangle = GetHandleRectangle();
			Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;

			bool isHovering = isHoveringOverHandle;
			isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));

			if (!isHovering && isHoveringOverHandle && Main.hasFocus)
				Main.PlaySound(12);

			if (background != null)
				DrawBar(spriteBatch, background, dimensions.ToRectangle(), backgroundStumpSize, Color.White);
			if (thumb != null)
				DrawBar(spriteBatch, thumb, handleRectangle, thumbStumpSize, Color.White * ((isDragging || isHoveringOverHandle) ? 1f : 0.85f));
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);
			if (evt.Target == this)
			{
				Rectangle handleRectangle = GetHandleRectangle();
				if (handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y)))
				{
					isDragging = true;
					dragYOffset = evt.MousePosition.Y - (float)handleRectangle.Y;
				}
				else
				{
					CalculatedStyle innerDimensions = GetInnerDimensions();
					float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - (handleRectangle.Height >> 1);

					if (smoothScroll)
						smoothViewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
					else
						viewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
				}
			}
		}
		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);
			isDragging = false;
		}
	}

	internal class GenericUIPanel : UIPanel
	{
		internal Texture2D panelTexture;

		internal Vector2 panelDimensions;

		public GenericUIPanel(Texture2D panelTexture, Vector2 panelDimensions)
		{
			this.panelTexture = panelTexture;
			this.panelDimensions = panelDimensions;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle panelRectangle = this.GetDimensions().ToRectangle();
			UIUtilities.DrawPanelBorders(spriteBatch, panelTexture, panelRectangle, (int)panelDimensions.X, (int)panelDimensions.Y, true);
		}
	}
}
