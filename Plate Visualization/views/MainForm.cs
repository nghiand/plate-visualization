﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Plate_Visualization
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Form name
        /// </summary>
        private const string FORM_NAME = "Plate Visualization";
        /// <summary>
        /// Graphic
        /// </summary>
        private Graphic graphic;
        /// <summary>
        /// Scheme
        /// </summary>
        private Scheme scheme;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initial
        /// </summary>
        private void Initial()
        {
            status.Text = "";
            SetToolItemsAvailability(false);
        }

        /// <summary>
        /// Load form
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            graphic = new Graphic(graph.CreateGraphics());
            Initial();
        }

        /// <summary>
        /// Set tool items availability
        /// </summary>
        /// <param name="enabled">Enabled</param>
        private void SetToolItemsAvailability(bool enabled)
        {
            selectNodeButton.Enabled = enabled;
            bondButton.Enabled = enabled;
            selectElementButton.Enabled = enabled;
            stiffnessButton.Enabled = enabled;
            saveStripButton.Enabled = enabled;
            saveAsStripButton.Enabled = enabled;
            view2D.Enabled = enabled;
            view3D.Enabled = enabled;
        }

        /// <summary>
        /// Create plate from NewPlateForm's input
        /// </summary>
        /// <param name="name">Scheme name</param>
        /// <param name="inputWidth">Elements along first axis</param>
        /// <param name="inputLength">Elements along second axis</param>
        public void CreatePlate(string name, List<Tuple<int, float>> inputWidth, List<Tuple<int, float>> inputLength)
        {
            if (inputWidth.Count == 0 || inputLength.Count == 0 || name == "")
            {
                MessageBox.Show("Некорректный ввод!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            scheme = new Scheme();
            scheme.Name = name;
            scheme.Plate = new Plate(inputWidth, inputLength, graph.Width, graph.Height);
            scheme.Loads = new List<Load>();
            scheme.Plate.Subscribe(this);
            ModifyScheme();

            InitiateTools();

            graphic.DrawScheme(scheme);
        }

        /// <summary>
        /// Initiate button tools
        /// </summary>
        private void InitiateTools()
        {
            selectElementButton.Enabled = true;
            selectNodeButton.Enabled = true;
            saveStripButton.Enabled = true;
            saveAsStripButton.Enabled = true;
            view2D.Enabled = true;
            view3D.Enabled = true;
            view2D.Checked = true;
            view3D.Checked = false;
        }

        /// <summary>
        /// Call when new scheme tool button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (NewPlateForm newForm = new NewPlateForm())
            {
                newForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Call when new scheme strip button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void newStripButton_Click(object sender, EventArgs e)
        {
            using (NewPlateForm newForm = new NewPlateForm())
            {
                newForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Call when exit tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Call when scroll mouse
        /// </summary>
        /// <param name="e">Mouse event</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            graph.Focus();
            if (scheme == null)
                return;
            if (graph.Focused == true && e.Delta != 0)
            {
                scheme.Plate.Zoom(e.Location, e.Delta > 0);
                ModifyScheme();
                graphic.DrawScheme(scheme);
            }
        }

        /// <summary>
        /// Starting mouse click point
        /// </summary>
        private PointF startingPoint = PointF.Empty;
        /// <summary>
        /// Check is panning
        /// </summary>
        private bool panning = false;

        /// <summary>
        /// Call when mouse down on graph
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Mouse event</param>
        private void graph_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control && e.Button == MouseButtons.Left)
            {
                panning = true;
                startingPoint = new PointF(e.Location.X, e.Location.Y);
            }

        }

        /// <summary>
        /// Call when mouse up on graph
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Mouse event</param>
        private void graph_MouseUp(object sender, MouseEventArgs e)
        {
            panning = false;
        }

        /// <summary>
        /// Call when mouse move on graph
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Mouse event</param>
        private void graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (panning)
            {
                PointF movingVector = new PointF(e.Location.X - startingPoint.X, e.Location.Y - startingPoint.Y);
                scheme.Plate.Move(movingVector);
                ModifyScheme();
                startingPoint = new PointF(e.Location.X, e.Location.Y);
                graphic.DrawScheme(scheme);
            }
            else if (scheme != null)
            {
                scheme.Plate.OnMouseMove(e);
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when mouse click on graph
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Mouse event</param>
        private void graph_MouseClick(object sender, MouseEventArgs e)
        {
            if (scheme != null && e.Button == MouseButtons.Left)
            {
                scheme.Plate.OnMouseClick(e);
            }
        }

        /// <summary>
        /// Call when windows size is changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Mouse event</param>
        private void graph_SizeChanged(object sender, EventArgs e)
        {
            if (scheme != null)
            {
                graphic = new Graphic(graph.CreateGraphics());
                graphic.DrawScheme(scheme);
            }
        }

        /// <summary>
        /// Call when mouse click on plateObject
        /// </summary>
        /// <param name="sender">Sender</param>
        public void plateObject_MouseClick(object sender)
        {
            if (selectNodeButton.Checked && sender is Node)
            {
                ((PlateObject)sender).Toggle();
                ModifyScheme();
            }
            else if (selectElementButton.Checked && sender is Element)
            {
                ((PlateObject)sender).Toggle();
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when mouse hover on plateObject
        /// </summary>
        /// <param name="sender">Sender</param>
        public void plateObject_MouseHover(object sender)
        {
            if (sender is Node)
            {
                Node node = (Node)sender;
                graphic.DrawNode(node, true);
                graphic.DrawLoads(scheme.Loads);
                status.Text = "Связи: (" + node.Bonds[0].ToString()
                    + ", " + node.Bonds[0].ToString()
                    + ", " + node.Bonds[0].ToString() + ")";
            }
            else if (sender is Element)
            {
                Element element = (Element)sender;
                graphic.DrawElement(element, true);
                graphic.DrawLoads(scheme.Loads);
                status.Text = "E = " + element.Stiffness.E
                    + ", H = " + element.Stiffness.H.ToString()
                    + ", V = " + element.Stiffness.V.ToString();
            }
        }

        /// <summary>
        /// Call when mouse leave plateObject
        /// </summary>
        /// <param name="sender">Sender</param>
        public void plateObject_MouseLeave(object sender)
        {
            if (sender is Node)
            {
                graphic.DrawNode((Node)sender);
                graphic.DrawLoads(scheme.Loads);
            }
            else if (sender is Element)
            {
                graphic.DrawElement((Element)sender);
                graphic.DrawLoads(scheme.Loads);
            }
            status.Text = "";
        }

        /// <summary>
        /// Call when an plateObject is selected
        /// </summary>
        /// <param name="sender">Sender</param>
        public void plateObject_Selected(object sender)
        {
            if (sender is Node)
            {
                graphic.DrawNode((Node)sender);
                graphic.DrawLoads(scheme.Loads);
                ModifyScheme();
            }
            else if (sender is Element)
            {
                graphic.DrawElement((Element)sender);
                graphic.DrawLoads(scheme.Loads);
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when an plateObject is deselected
        /// </summary>
        /// <param name="sender"></param>
        public void plateObject_Deselected(object sender)
        {
            if (sender is Node)
            {
                graphic.DrawNode((Node)sender);
                graphic.DrawLoads(scheme.Loads);
                ModifyScheme();
            }
            else if (sender is Element)
            {
                graphic.DrawElement((Element)sender);
                graphic.DrawLoads(scheme.Loads);
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when select element button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void selectElementButton_Click(object sender, EventArgs e)
        {
            if (selectElementButton.Checked)
            {
                selectElementButton.Checked = false;
                stiffnessButton.Enabled = false;
            }
            else
            {
                selectElementButton.Checked = true;
                stiffnessButton.Enabled = true;
                selectNodeButton.Checked = false;
                bondButton.Enabled = false;
            }
        }

        /// <summary>
        /// Call when stiffness button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void stiffnessButton_Click(object sender, EventArgs e)
        {
            List<Element> selectingElements = scheme.Plate.SelectingElements();
            Stiffness stiffness = new Stiffness();
            if (selectingElements.Count > 0)
            {
                stiffness = selectingElements[0].Stiffness;
            }
            for (int i = 1; i < selectingElements.Count; i++)
            {
                if (!selectingElements[i].Stiffness.Equals(selectingElements[i - 1].Stiffness))
                {
                    stiffness = new Stiffness();
                    break;
                }
            }
            using (StiffnessForm stiffnessForm = new StiffnessForm(stiffness))
            {
                stiffnessForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Call when select node button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void selectNodeButton_Click(object sender, EventArgs e)
        {
            if (selectNodeButton.Checked)
            {
                selectNodeButton.Checked = false;
                bondButton.Enabled = false;
            }
            else
            {
                selectNodeButton.Checked = true;
                bondButton.Enabled = true;
                selectElementButton.Checked = false;
                stiffnessButton.Enabled = false;
            }
        }

        /// <summary>
        /// Call when select node button checked is changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void selectNodeButton_CheckedChanged(object sender, EventArgs e)
        {
            if (selectNodeButton.Checked)
            {
            }
            else
            {
                scheme.Plate.DeselectNodes();
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when bond button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void bondButton_Click(object sender, EventArgs e)
        {
            List<Node> selectingNodes = scheme.Plate.SelectingNodes();
            List<int> bonds = new List<int>(3) { 0, 0, 0 };
            if (selectingNodes.Count > 0)
            {
                bonds = selectingNodes[0].Bonds;
            }
            for (int i = 1; i < selectingNodes.Count; i++)
            {
                if (!selectingNodes[i].Bonds.Equals(selectingNodes[i - 1].Bonds))
                {
                    bonds = new List<int>(3) { 0, 0, 0 };
                    break;
                }
            }
            using (NodeBondsForm bondsForm = new NodeBondsForm(bonds))
            {
                bondsForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Set bonds to nodes
        /// </summary>
        /// <param name="bonds">Bonds</param>
        public void SetBonds(List<int> bonds)
        {
            scheme.Plate.SetBonds(bonds);
            ModifyScheme();
        }

        /// <summary>
        /// Set stiffness to elements
        /// </summary>
        /// <param name="s">Stiffness</param>
        public void SetStiffness(Stiffness s)
        {
            scheme.Plate.SetStiffness(s);
            ModifyScheme();
        }

        /// <summary>
        /// Call when select element button checked is changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void selectElementButton_CheckedChanged(object sender, EventArgs e)
        {
            if (selectElementButton.Checked)
            {
            }
            else
            {
                scheme.Plate.DeselectElements();
                ModifyScheme();
            }
        }

        /// <summary>
        /// Call when view2D button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void view2D_Click(object sender, EventArgs e)
        {
            if (view2D.Checked == false)
            {
                view2D.Checked = true;
                view3D.Checked = false;
                if (scheme == null)
                    return;
                scheme.Plate.TranslateTo2D(graph.Width, graph.Height);
                ModifyScheme();
                graphic.DrawScheme(scheme);
            }
        }

        /// <summary>
        /// Call when view3D button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void view3D_Click(object sender, EventArgs e)
        {
            if (view3D.Checked == false)
            {
                view3D.Checked = true;
                view2D.Checked = false;
                if (scheme == null)
                    return;
                scheme.Plate.TranslateTo3D(graph.Width, graph.Height);
                ModifyScheme();
                graphic.DrawScheme(scheme);
            }
        }

        /// <summary>
        /// Set loads on nodes
        /// </summary>
        /// <param name="P">Weight</param>
        public void SetLoads(float P)
        {
            List<Node> selectingNodes = scheme.Plate.SelectingNodes();
            foreach (Node n in selectingNodes)
            {
                Load l = new Load(P, n);
                bool found = false;
                for (int i = 0; i < scheme.Loads.Count; i++)
                {
                    if (scheme.Loads[i].Position == n)
                    {
                        scheme.Loads[i] = l;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    scheme.Loads.Add(l);
                }
                ModifyScheme();
                graphic.DrawScheme(scheme);
            }
        }

        /// <summary>
        /// Call when load button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void loadButton_Click(object sender, EventArgs e)
        {
            List<Node> selectingNodes = scheme.Plate.SelectingNodes();
            using (views.NewLoadForm loadForm = new views.NewLoadForm())
            {
                loadForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Display save file dialof
        /// </summary>
        /// <param name="type">File type to save</param>
        /// <returns></returns>
        private string DisplaySaveFileDialog(string type)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = type;
            saveFileDialog.Title = "Сохранить в файл";
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        /// <summary>
        /// Display open file dialog
        /// </summary>
        /// <param name="type">File type to show</param>
        /// <returns></returns>
        private string DisplayOpenFileDialog(string type)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = type;
            openFileDialog.Title = "Открыть файл";
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        /// <summary>
        /// Save scheme to file
        /// </summary>
        private void SaveSchemeToFile()
        {
            if (scheme == null)
                return;
            if (scheme.Filename == "")
            {
                string filename = DisplaySaveFileDialog("Plate Visualization File|*.pv");
                if (filename != "")
                {
                    scheme.SaveToFile(filename);
                }
            }
            else
            {
                scheme.SaveFile();
            }
        }

        /// <summary>
        /// Open scheme from file
        /// </summary>
        private void OpenScheme()
        {
            string filename = DisplayOpenFileDialog("Plate Visualization File|*.pv");
            if (filename != "")
            {
                bool open = true;
                if (scheme != null && scheme.IsModified)
                {
                    DialogResult result = MessageBox.Show("Схема не сохранена! Сохранить?", "Предупреждение",
                                          MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveSchemeToFile();
                        graphic.Clear();
                        scheme = null;
                    }
                    else if (result == DialogResult.No)
                    {
                        graphic.Clear();
                        scheme = null;
                    }
                    else
                    {
                        open = false;
                    }
                }
                if (open)
                {
                    Scheme new_scheme = new Scheme();
                    new_scheme.OpenFromFile(filename);
                    scheme = new_scheme;
                    graphic.DrawScheme(scheme);
                    scheme.Plate.Subscribe(this);

                    InitiateTools();

                    bool check = false;
                    foreach (Element element in scheme.Plate.Elements)
                    {
                        if (element.State == State.Selecting)
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        selectElementButton.Checked = true;
                    }
                    else
                    {
                        selectElementButton.Checked = false;
                        foreach (Node node in scheme.Plate.Nodes)
                        {
                            if (node.State == State.Selecting)
                            {
                                check = true;
                                break;
                            }
                        }
                        if (check)
                        {
                            selectNodeButton.Checked = true;
                        }
                        else
                        {
                            selectNodeButton.Checked = false;
                        }
                    }
                    if (scheme.Plate.Mode2D == true)
                    {
                        view2D.Checked = true;
                        view3D.Checked = false;
                    }
                    else
                    {
                        view2D.Checked = false;
                        view3D.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Call when save tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSchemeToFile();
        }

        /// <summary>
        /// Call when save strip button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void saveStripButton_Click(object sender, EventArgs e)
        {
            SaveSchemeToFile();
        }

        /// <summary>
        /// Call when export tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scheme == null)
                return;
            string filename = DisplaySaveFileDialog("Text File|*.txt");

            if (filename != "")
            {
                scheme.Export(filename);
            }
        }

        /// <summary>
        /// Call when close tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scheme.IsModified == true)
            {
                DialogResult result = MessageBox.Show("Схема не сохранена! Сохранить?", "Предупреждение",
                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveSchemeToFile();
                    graphic.Clear();
                    scheme = null;
                }
                else if (result == DialogResult.No)
                {
                    graphic.Clear();
                    scheme = null;
                }
                else
                {

                }
            } 

        }

        /// <summary>
        /// Call when open tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenScheme();
        }

        /// <summary>
        /// Call when save as tool strip is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSchemeToFile();
        }

        /// <summary>
        /// Call when scheme is modified
        /// </summary>
        private void ModifyScheme()
        {
            scheme.IsModified = true;
            Name = FORM_NAME + " *";
        }

        /// <summary>
        /// Call when open strip button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void openStripButton_Click(object sender, EventArgs e)
        {
            OpenScheme();
        }

        /// <summary>
        /// Call when save as strip button is clicked
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void saveAsStripButton_Click(object sender, EventArgs e)
        {
            SaveSchemeToFile();
        }
    }
}
