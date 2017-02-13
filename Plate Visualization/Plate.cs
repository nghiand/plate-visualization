﻿using Plate_Visualization.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plate_Visualization
{
    public enum State { Normal, Selecting, Selected }

    public delegate void MouseHoverHandler(object sender);
    public delegate void MouseLeaveHandler(object sender);
    public delegate void MouseClickHandler(object sender);

    class Plate
    {
        private List<Element> elements;
        public List<Element> Elements
        {
            get
            {
                return this.elements;
            }
        }
        private List<Node> nodes;
        public List<Node> Nodes
        {
            get
            {
                return this.nodes;
            }
        }
        private int width;
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
        private int length;
        public int Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = Length;
            }
        }

        public Plate(List<Tuple<int, float>> widths, List<Tuple<int, float>> lengths, int graphicWidth, int graphicLength, Graphic graphic, bool modified = true)
        {
            nodes = new List<Node>();
            elements = new List<Element>();
            float lengthX = 0.8f * graphicWidth;
            float lengthY = 0.8f * graphicLength;

            float sumX = 0;
            this.width = 0;
            foreach (Tuple<int, float> elementX in widths)
            {
                sumX += elementX.Item2 * elementX.Item1;
                this.width += elementX.Item1;
            }
            float sumY = 0;
            this.length = 0;
            foreach (Tuple<int, float> elementY in lengths)
            {
                sumY += elementY.Item2 * elementY.Item1;
                this.length += elementY.Item1;
            }

            float unit = Math.Min(lengthX / sumX, lengthY / sumY);

            lengthX = sumX * unit;
            lengthY = sumY * unit;

            float paddingX = (1.0f * graphicWidth - lengthX) / 2;
            float paddingY = (1.0f * graphicLength - lengthY) / 2;

            float x = paddingX;
            float y = paddingY;
            nodes.Add(new Node(nodes.Count, (int)Math.Ceiling(x), (int)Math.Ceiling(y)));
            foreach (Tuple<int, float> elementX in widths)
            {
                for (int i = 0; i < elementX.Item1; i++)
                {
                    x += elementX.Item2 * unit;
                    nodes.Add(new Node(nodes.Count, (int)Math.Ceiling(x), (int)Math.Ceiling(y)));
                }
            }

            foreach (Tuple<int, float> elementY in lengths)
            {
                for (int j = 0; j < elementY.Item1; j++)
                {
                    x = paddingX;
                    y += elementY.Item2 * unit;
                    nodes.Add(new Node(nodes.Count, (int)Math.Ceiling(x), (int)Math.Ceiling(y)));
                    foreach (Tuple<int, float> elementX in widths)
                    {
                        for (int i = 0; i < elementX.Item1; i++)
                        {
                            x += elementX.Item2 * unit;
                            nodes.Add(new Node(nodes.Count, (int)Math.Ceiling(x), (int)Math.Ceiling(y)));
                        }
                    }
                }
            }

            int nodeId = 0;
            int currentWidth = 0, currentLength = 0;
            int maxWidth = widths[0].Item1, maxLength = lengths[0].Item1;
            for (int i = 0; i < this.length; i++)
            {
                if (i >= maxLength)
                {
                    currentLength++;
                    maxLength += lengths[currentLength].Item1;
                }
                currentWidth = 0;
                maxWidth = widths[0].Item1;
                for (int j = 0; j < this.width; j++)
                {
                    if (j >= maxWidth)
                    {
                        currentWidth++;
                        maxWidth += widths[currentWidth].Item1;
                    }
                    elements.Add(new Element(i * this.width + j,
                        widths[currentWidth].Item1, lengths[currentLength].Item1,
                        nodes[nodeId],
                        nodes[nodeId + 1],
                        nodes[nodeId + this.width + 1],
                        nodes[nodeId + this.width + 2]));
                    nodeId++;
                }
                nodeId++;
            }
            this.modified = modified;

            Subscribe(graphic);
        }

        private void Subscribe(Graphic graphic)
        {
            foreach (Node n in nodes)
            {
                n.MouseHover += graphic.MouseHover;
                n.MouseLeave += graphic.MouseLeave;
                n.MouseClick += graphic.MouseClick;
            }
            foreach (Element e in elements)
            {
                e.MouseHover += graphic.MouseHover;
                e.MouseLeave += graphic.MouseLeave;
            }
        }

        public void Zoom(Point location, bool zoomIn)
        {
            float factor = 1.0f;
            if (zoomIn)
            {
                factor += MathHelper.SCALE_FACTOR;
            }
            else
            {
                factor -= MathHelper.SCALE_FACTOR;
            }
            float x0 = location.X;
            float y0 = location.Y;

            for (int i = 0; i < this.nodes.Count; i++)
            {
                this.nodes[i].Point = new Point(
                    (int)Math.Ceiling(factor * (this.nodes[i].X - x0) + x0), 
                    (int)Math.Ceiling(factor * (this.nodes[i].Y - y0) + y0));
            }
        }

        public void Move(Point movingVector)
        {
            for (int i = 0; i < this.nodes.Count; i++)
            {
                this.nodes[i].Point = new Point(
                    this.nodes[i].X + movingVector.X,
                    this.nodes[i].Y + movingVector.Y);
            }
        }

        private bool modified;
        public bool Modified
        {
            get
            {
                return this.modified;
            }
            set
            {
                modified = value;
            }
        }

        public void MouseMove(Point location)
        {
            foreach (Node n in nodes)
            {
                n.MouseMove(location);
            }
            foreach (Element e in elements)
            {
                e.MouseMove(location);
            }
        }

        public void MouseClickOnNode()
        {
            foreach (Node n in nodes)
            {
                if (n.Hovered)
                {
                    n.Selecting();
                    break;
                }
            }
        }
    }
}
