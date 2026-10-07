using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OctTreeVoxel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mesh tool;
        readonly OctTreeNode oct;
        public Array StepsToMove { get; set; } = new int[] { 1, 2, 3, 4, 5, 10, 15, 20, 25 };
        public int StepToMove { get; set; }
        private Vector3D _directionVect;
        public MainWindow()
        {
            _directionVect = new Vector3D(0, 0, StepToMove);
            InitializeComponent();
            DataContext = this;
            design1.Renderer = devDept.Eyeshot.Control.rendererType.Direct3D;
            BoxSize boxSize = new BoxSize();
            boxSize.BoxMin = new PointD3(0, 0, 0);
            boxSize.BoxMax = new PointD3(50, 50, 50);
            oct = new OctTreeNode(boxSize) { Design = design1 };
            oct.Initialize();
            LoadTool();
            design1.Invalidate();
        }
        private void LoadTool()
        {
            tool = Mesh.CreateBox(10, 10, 10);
            tool.Translate(25, 25, 55);
            design1.Entities.Add(tool);
            design1.Invalidate();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Content.ToString();
            switch (buttonName)
            {
                case "X":
                    _directionVect = new Vector3D(StepToMove, 0, 0); break;
                case "Y":
                    _directionVect = new Vector3D(0, StepToMove, 0); break;
                case "Z":
                    _directionVect = new Vector3D(0, 0, StepToMove); break;
            }
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            Vector3D translationVect = _directionVect;
            Button button = sender as Button;
            if ((string)button.Content == "Minus")
                translationVect *= -1;

            tool.Translate(translationVect);
            tool.Regen(new RegenParams(1, design1));
            tool.Compile(new CompileParams(design1));
            design1.Invalidate();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            StepToMove = (int)StepsToMove.GetValue(comboBox.SelectedIndex);
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            oct.UpdateVoxels(tool);
            design1.Invalidate();
        }

        private void Move_Cut_Click(object sender, RoutedEventArgs e)
        {
            Vector3D translationVect = _directionVect;
            Button button = sender as Button;
            if ((string)button.Content == "CutMinus")
                translationVect *= -1;

            tool.Translate(translationVect);
            tool.Regen(new RegenParams(1, design1));
            tool.Compile(new CompileParams(design1));
            oct.UpdateVoxels(tool);
            design1.Invalidate();
        }
    }
}
