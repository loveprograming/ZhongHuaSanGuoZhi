﻿namespace TabListPlugin
{
    using GameFreeText;
    using GameGlobal;
    using GameObjects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public class Column
    {
        internal FreeTextList ColumnTextList;
        public string DisplayName;
        internal bool Editable = false;
        public int ID;
        public bool IsNumber;
        public int MinWidth;
        public string Name;
        public float Scale = 1f;
        public bool SmallToBig;
        public int ItemID = -1;
        public int DetailLevel;
        public bool CountToDisplay = true;
        private TabListInFrame tabList;
        internal FreeText Text;

        internal Column(TabListInFrame tabList)
        {
            this.tabList = tabList;
            this.Text = new FreeText(tabList.ColumnTextBuilder);
            this.Text.TextColor = tabList.ColumnTextColor;
            this.Text.Align = tabList.ColumnTextAlign;
            this.Text.Position = new Rectangle(0, 0, 0, tabList.columnheaderHeight);
        }

        public void AdjustRowRectangles(List<Rectangle> rowRectangles)
        {
            for (int i = 0; i < rowRectangles.Count; i++)
            {
                rowRectangles[i] = new Rectangle(rowRectangles[i].X, this.ColumnTextList.DisplayPosition(i).Y, rowRectangles[i].Width, this.tabList.rowHeight);
            }
        }

        public void ClearData()
        {
            this.ColumnTextList.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.DisplayPosition.Right > this.tabList.VisibleLowerClient.Right)
            {
                if (this.DisplayPosition.Left < this.tabList.VisibleLowerClient.Right)
                {
                    spriteBatch.Draw(this.tabList.rightArrowTexture, StaticMethods.LeftRectangle(this.DisplayPosition, new Rectangle(0, 0, this.tabList.rightArrowTexture.Width, this.tabList.rightArrowTexture.Height)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
                }
            }
            else if (this.DisplayPosition.Left < this.tabList.VisibleLowerClient.Left)
            {
                if (this.DisplayPosition.Right > this.tabList.VisibleLowerClient.Left)
                {
                    spriteBatch.Draw(this.tabList.leftArrowTexture, StaticMethods.RightRectangle(this.DisplayPosition, new Rectangle(0, 0, this.tabList.leftArrowTexture.Width, this.tabList.leftArrowTexture.Height)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
                }
            }
            else
            {
                Rectangle? sourceRectangle = null;
                spriteBatch.Draw(this.tabList.columnheaderTexture, this.DisplayPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
                sourceRectangle = null;
                spriteBatch.Draw(this.tabList.columnspliterTexture, this.SpliterPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.35f);
                this.Text.Draw(spriteBatch, 0.3499f);
                for (int i = 0; i < this.ColumnTextList.Count; i++)
                {
                    if (((this.ColumnTextList.DisplayPosition(i).Bottom <= this.tabList.VisibleLowerClient.Bottom) && (this.ColumnTextList.DisplayPosition(i).Top >= this.DisplayPosition.Bottom)) && (this.ColumnTextList[i].TextTexture != null))
                    {
                        if (this.Editable)
                        {
                            sourceRectangle = null;
                            spriteBatch.Draw(this.ColumnTextList[i].TextTexture, StaticMethods.CenterRectangle(this.ColumnTextList.DisplayPosition(i), new Rectangle(0, 0, this.tabList.checkboxWidth, this.tabList.checkboxWidth)), sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.3499f);
                        }
                        else
                        {
                            this.ColumnTextList.Draw(spriteBatch, i, 0.3499f);
                        }
                    }
                }
            }
        }

        public void MoveHorizontal(int offset)
        {
            this.Text.DisplayOffset = new Point(this.Text.DisplayOffset.X + offset, this.Text.DisplayOffset.Y);
            this.ColumnTextList.DisplayOffset = new Point(this.ColumnTextList.DisplayOffset.X + offset, this.ColumnTextList.DisplayOffset.Y);
        }

        public void MoveVertical(int offset)
        {
            Rectangle rectangle = this.ColumnTextList.DisplayPosition(0);
            Rectangle rectangle2 = this.ColumnTextList.DisplayPosition(this.ColumnTextList.Count - 1);
            if ((rectangle.Top + offset) > (this.DisplayPosition.Bottom + 1))
            {
                offset = (this.DisplayPosition.Bottom + 1) - rectangle.Top;
            }
            else if ((rectangle2.Bottom + offset) < this.tabList.VisibleLowerClient.Bottom)
            {
                offset = this.tabList.VisibleLowerClient.Bottom - rectangle2.Bottom;
            }
            if (offset != 0)
            {
                this.ColumnTextList.DisplayOffset = new Point(this.ColumnTextList.DisplayOffset.X, this.ColumnTextList.DisplayOffset.Y + offset);
            }
        }

        public string GetDataString(int index)
        {
            if (this.Name.Equals("HasSkill"))
            {
                return ((Person)this.tabList.gameObjectList[index]).HasSkill(this.ItemID) ? "○" : "×";
            }

            if (this.Name.Equals("HasStunt"))
            {
                return ((Person)this.tabList.gameObjectList[index]).HasStunt(this.ItemID) ? "○" : "×";
            }
            if (this.Name.Equals("TitleName"))
            {
                return ((Person) this.tabList.gameObjectList[index]).TitleName(this.ItemID);
            }
            if (this.Name.Equals("HasInfluenceKind"))
            {
                return ((Person)this.tabList.gameObjectList[index]).HasInfluenceKind(this.ItemID) ? "○" : "×";
            }
            if (this.Name.Equals("InfluenceKindValueByTreasure"))
            {
                return ((Person)this.tabList.gameObjectList[index]).InfluenceKindValueByTreasure(this.ItemID).ToString();
            }

            object obj = StaticMethods.GetPropertyValue(this.tabList.gameObjectList[index], this.Name);
            String s;
            if (obj is bool)
            {
                s = ((bool)obj) ? "○" : "×";
            }
            else
            {
                s = obj.ToString();
            }
            return s;
        }

        public void ReCalculate(int top, ref int previousRight)
        {
            if (!this.Visible) return;
            this.Text.DisplayOffset = Point.Zero;
            int width = this.Text.Width;
            if (width < this.MinWidth)
            {
                width = this.MinWidth;
            }
            this.Text.Position = new Rectangle(previousRight + 1, top, width, this.Text.Position.Height);
            previousRight = this.Text.Position.Right + this.tabList.columnspliterWidth;
            if (this.tabList.gameObjectList != null)
            {
                int num2;
                if (this.ColumnTextList.Count == 0)
                {
                    for (num2 = 0; num2 < this.tabList.gameObjectList.Count; num2++)
                    {
                        string s = GetDataString(num2);
                        this.ColumnTextList.AddText(s);
                    }
                    this.ColumnTextList.ResetAllTextTextures();
                }
                for (num2 = 0; num2 < this.ColumnTextList.Count; num2++)
                {
                    this.ColumnTextList[num2].MaxWidth = this.Text.Position.Width;
                    this.ColumnTextList[num2].Position = new Rectangle(this.Text.Position.X, (this.Text.Position.Bottom + 1) + (num2 * this.tabList.rowHeight), this.Text.Position.Width, this.tabList.rowHeight);
                }
                this.ColumnTextList.ResetAllAlignedPositions();
                if (this.Editable)
                {
                    this.ResetEditableTextures();
                }
            }
        }

        public void ResetAllTextures()
        {
            if (!this.Visible) return;
            int num;
            if (this.Editable)
            {
                for (num = 0; num < this.tabList.gameObjectList.Count; num++)
                {
                    if ((bool) StaticMethods.GetPropertyValue(this.tabList.gameObjectList[num], this.Name))
                    {
                        this.ColumnTextList[num].TextTexture = this.tabList.MultiSelecting ? this.tabList.checkboxSelectedTexture : this.tabList.roundcheckboxSelectedTexture;
                    }
                    else
                    {
                        this.ColumnTextList[num].TextTexture = this.tabList.MultiSelecting ? this.tabList.checkboxTexture : this.tabList.roundcheckboxTexture;
                    }
                }
                this.ColumnTextList.ResetAllAlignedPositions();
            }
            else
            {
                for (num = 0; num < this.tabList.gameObjectList.Count; num++)
                {
                    string s = this.GetDataString(num);
                    this.ColumnTextList[num].Text = s;
                }
                this.ColumnTextList.ResetAllTextTextures();
                this.ColumnTextList.ResetAllAlignedPositions();
            }
        }

        public void ResetEditableTextures()
        {
            if (this.Editable)
            {
                for (int i = 0; i < this.tabList.gameObjectList.Count; i++)
                {
                    if ((bool) StaticMethods.GetPropertyValue(this.tabList.gameObjectList[i], this.Name))
                    {
                        this.ColumnTextList[i].TextTexture = this.tabList.MultiSelecting ? this.tabList.checkboxSelectedTexture : this.tabList.roundcheckboxSelectedTexture;
                    }
                    else
                    {
                        this.ColumnTextList[i].TextTexture = this.tabList.MultiSelecting ? this.tabList.checkboxTexture : this.tabList.roundcheckboxTexture;
                    }
                }
                this.ColumnTextList.ResetAllAlignedPositions();
            }
        }

        internal Rectangle DisplayPosition
        {
            get
            {
                return this.Text.DisplayPosition;
            }
        }

        internal Rectangle SpliterPosition
        {
            get
            {
                return new Rectangle(this.DisplayPosition.Right + 1, this.DisplayPosition.Y, this.tabList.columnspliterWidth, this.tabList.columnspliterHeight);
            }
        }

        public bool Visible
        {
            get
            {
                return this.DetailLevel <= GlobalVariables.TabListDetailLevel;
            }
        }
    }
}

