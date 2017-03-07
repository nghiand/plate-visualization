﻿using System.Windows.Forms;

namespace Plate_Visualization
{
    public enum State { Normal, Selecting, Selected }

    public delegate void MouseHoverHandler(object sender);
    public delegate void MouseLeaveHandler(object sender);
    public delegate void MouseClickHandler(object sender);

    abstract class PlateObject
    {
        public State State
        {
            get; set;
        }
        public bool Hovered
        {
            get; set;
        }
        public event MouseHoverHandler MouseHover;
        public event MouseLeaveHandler MouseLeave;
        public event MouseClickHandler MouseClick;

        abstract public bool IsOnHover(MouseEventArgs e);
        abstract public bool IsSelected();

        public void OnMouseClick(MouseEventArgs e)
        {
            MouseClick?.Invoke(this);
        }

        public void OnMouseHover(MouseEventArgs e)
        {
            if (Hovered == false)
            {
                Hovered = true;
                MouseHover?.Invoke(this);
            }
        }

        public void OnMouseLeave(MouseEventArgs e)
        {
            if (Hovered == true)
            {
                Hovered = false;
                MouseLeave?.Invoke(this);
            }
        }

        public void Select()
        {
            State = State.Selecting;
        }

        public void Deselect()
        {
            if (IsSelected())
                State = State.Selected;
            else
                State = State.Normal;
        }
    }
}