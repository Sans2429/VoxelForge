using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
namespace Volvex
{
    public class Stock
    {
        private const string CubeStr = "Cube";
        private readonly double _height,_width,_depth;
        private Design _design;
        public Stock(double height, double width,double depth,Design design)
        {
            _height = height;
            _width = width;
            _depth = depth;
            _design = design;
            CreateCube();
        }

        private void CreateCube()
        {
            Brep cube = Brep.CreateBox(1, 1, 1);
            Block block = new Block(CubeStr);
            _design.Blocks.Add(block);
        }

        private void GetCube() => new BlockReference(CubeStr);


    }
}
