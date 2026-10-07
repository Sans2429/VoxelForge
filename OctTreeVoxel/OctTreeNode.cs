using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace OctTreeVoxel
{
    public class OctTreeNode
    {
        private const int MaxChildCount = 8;
        private bool _isLeaf;
        private Entity _voxel;
        private readonly bool _isRoot;

        internal OctTreeNode Parent;
        internal OctTreeNode Root;
        public Design Design;
        public BoxSize BoxSize { get; set; }
        public OctTreeNode[] Children { get; set; }
        public OctTreeNode(BoxSize boxSize) : this(boxSize, null)
        {
            _isRoot = true;
        }
        protected OctTreeNode(BoxSize boxSize, OctTreeNode parentNode)
        {
            double length = PointD3.Distance(boxSize.BoxMin, boxSize.BoxMax);
            _isLeaf = length <= 0.5;
            BoxSize = boxSize;
            Parent = parentNode;
            Root = parentNode?.Root ?? this;
        }
        public static Color GetRandomMidToneColor(int opacity = 255)
        {
            int startRange = 100;
            int endRange = startRange + 50;
            Random rand = new Random(DateTime.Now.Millisecond);
            int r = rand.Next(startRange, endRange);
            int g = rand.Next(startRange, endRange);
            int b = rand.Next(startRange, endRange);

            return Color.FromArgb(opacity, r, g, b);
        }
        private bool _isVoxelUpdated;
        public void Initialize()
        {
            if (_isVoxelUpdated) return;
            _isVoxelUpdated = true;

            AddEntity();
        }
        public void Initialize1()
        {
            if (_isVoxelUpdated) return;
            _isVoxelUpdated = true;

            AddEntity();
            Root.Design.Entities.AddRange(TempHolder.This.NewEntities);
            TempHolder.This.Reset();
        }
        public void UpdateVoxels(Entity entity)
        {
            BoxSize toolBoxSize = new BoxSize();
            toolBoxSize.BoxMin = new PointD3(entity.BoxMin);
            toolBoxSize.BoxMax = new PointD3(entity.BoxMax);

            if (!IsCollide(toolBoxSize)) return;

            UpdateChilds(toolBoxSize);
        }
        public void UpdateVoxel1(Entity entity)
        {
            BoxSize toolBoxSize = new BoxSize();
            toolBoxSize.BoxMin = new PointD3(entity.BoxMin);
            toolBoxSize.BoxMax = new PointD3(entity.BoxMax);

            if (!IsCollide(toolBoxSize)) return;

            UpdateChilds(toolBoxSize);

            Root.Design.Entities.AddRange(TempHolder.This.NewEntities);
            Root.Design.Entities.Remove(TempHolder.This.OldEntities);
            TempHolder.This.Reset();
        }
        private bool IsCollide(BoxSize toolBoxSize)
        {
            return Utility.DoOverlapOrTouch
                (
                BoxSize.BoxMin.GetGeometryPoint(),
                BoxSize.BoxMax.GetGeometryPoint(),
                toolBoxSize.BoxMin.GetGeometryPoint(),
                toolBoxSize.BoxMax.GetGeometryPoint()
                );

        }
        private bool _isRemoved;
        private void TrySubDivide()
        {
            if (Children is null && !_isLeaf)
            {
                Children = SubDivide();
                RemoveVoxel();
            }
        }
        private bool ShouldRemove(BoxSize toolBoxSize)
        {
            if (Children is null && (IsInside(toolBoxSize) || (_isLeaf && IsCollide(toolBoxSize))))
            {
                RemoveVoxelWithFlag();
                return true;
            }
            return false;
        }
        private bool IsChildRemoved()
        {
            return _isRemoved;
        }
        private void RemoveVoxelWithFlag()
        {
            _isRemoved = true;
            RemoveVoxel();
        }
        private void UpdateChilds1(BoxSize toolBoxSize)
        {
            if (_isRemoved)
                return;
            TrySubDivide();
            if (ShouldRemove(toolBoxSize)) return;

            if (Children is null)
            {
                _isLeaf = true;
                return;
            }

            foreach (var child in Children)
            {
                if (_isRemoved)
                    continue;
                if (child.IsInside(toolBoxSize))
                {
                    _isRemoved = true;
                    RemoveVoxel();
                    continue;
                }
                else if (!child.IsCollide(toolBoxSize))
                {
                    child.AddEntity();
                    continue;
                }

                child.UpdateChilds(toolBoxSize);
            }
        }
        private void UpdateChilds(BoxSize toolBoxSize)
        {
            if (_isRemoved || ShouldRemove(toolBoxSize)) return;

            TrySubDivide();

            if (Children is null)
            {
                _isLeaf = true;
                return;
            }
            foreach (var child in Children)
            {
                if (child.IsChildRemoved())
                    continue;
                else if (child.IsInside(toolBoxSize) && child.Children is null)
                {
                    child.RemoveVoxelWithFlag();
                    continue;
                }
                else if (!child.IsCollide(toolBoxSize) && child.Children is null)
                {
                    child.AddEntity();
                    continue;
                }

                child.UpdateChilds(toolBoxSize);
            }
        }
        private void UpdateChilds2(BoxSize toolBoxSize)
        {
            if (_isRemoved || ShouldRemove(toolBoxSize)) return;

            TrySubDivide();

            if (Children is null)
            {
                _isLeaf = true;
                return;
            }
            TraverseChilds(toolBoxSize);
        }
        private void TraverseChilds(BoxSize toolBoxSize)
        {
            if (_isRoot)
                Parallel.ForEach(Children, (child) =>
                {
                    CheckChilds(child, toolBoxSize);
                });
            else
            {
                foreach (var child in Children)
                    CheckChilds(child, toolBoxSize);
            }
        }
        private void CheckChilds(OctTreeNode child, BoxSize toolBoxSize)
        {
            if (child.IsChildRemoved())
                return;
            else if (child.IsInside(toolBoxSize) && child.Children is null)
            {
                child.RemoveVoxelWithFlag();
                return;
            }
            else if (!child.IsCollide(toolBoxSize) && child.Children is null)
            {
                child.AddEntity();
                return;
            }

            child.UpdateChilds(toolBoxSize);
        }
        private void RemoveVoxel()
        {
            if (_voxel is null) return;
            bool isRemoved = Root.Design.Entities.Remove(_voxel);
            _voxel = null;
        }
        private void RemoveVoxel1()
        {
            if (_voxel is null) return;
            TempHolder.This.NewEntities.Add(_voxel);
            _voxel = null;
        }
        private void AddEntity()
        {
            if (_voxel != null) return;
            _voxel = GetVoxel();
            _voxel.Translate(BoxSize.BoxMin.GetVectorFromOrigin());
            Root.Design.Entities.Add(_voxel, GetRandomMidToneColor());
        }
        private void AddEntity1()
        {
            if (_voxel != null) return;
            _voxel = GetVoxel();
            _voxel.Translate(BoxSize.BoxMin.GetVectorFromOrigin());
            _voxel.ColorMethod = colorMethodType.byEntity;
            _voxel.Color = GetRandomMidToneColor();
            TempHolder.This.NewEntities.Add(_voxel);
        }
        private bool IsInside(BoxSize toolBoxSize)
        {
            return (toolBoxSize.BoxMin.X <= BoxSize.BoxMin.X && BoxSize.BoxMax.X <= toolBoxSize.BoxMax.X) &&
                    (toolBoxSize.BoxMin.Y <= BoxSize.BoxMin.Y && BoxSize.BoxMax.Y <= toolBoxSize.BoxMax.Y) &&
                    (toolBoxSize.BoxMin.Z <= BoxSize.BoxMin.Z && BoxSize.BoxMax.Z <= toolBoxSize.BoxMax.Z);
        }
        private OctTreeNode[] SubDivide()
        {
            PointD3 center = PointD3.Center(BoxSize.BoxMin, BoxSize.BoxMax);

            BoxSize firstQuadrant = new BoxSize()
            {
                BoxMax = BoxSize.BoxMax,
                BoxMin = center,
            };
            BoxSize secondQuadrant = firstQuadrant.Clone();
            secondQuadrant.BoxMax.Y = center.Y;
            secondQuadrant.BoxMin.Y = BoxSize.BoxMin.Y;

            BoxSize thirdQuadrant = new BoxSize()
            {
                BoxMax = center,
                BoxMin = BoxSize.BoxMin,
            };
            thirdQuadrant.BoxMax.Z = BoxSize.BoxMax.Z;
            thirdQuadrant.BoxMin.Z = center.Z;

            BoxSize fourthQuadrant = firstQuadrant.Clone();
            fourthQuadrant.BoxMax.X = center.X;
            fourthQuadrant.BoxMin.X = BoxSize.BoxMin.X;



            BoxSize fifthQuadrant = firstQuadrant.Clone();
            fifthQuadrant.BoxMax.Z = center.Z;
            fifthQuadrant.BoxMin.Z = BoxSize.BoxMin.Z;

            BoxSize sixthQuadrant = secondQuadrant.Clone();
            sixthQuadrant.BoxMax.Z = center.Z;
            sixthQuadrant.BoxMin.Z = BoxSize.BoxMin.Z;

            BoxSize seventhQuadrant = thirdQuadrant.Clone();
            seventhQuadrant.BoxMax.Z = center.Z;
            seventhQuadrant.BoxMin.Z = BoxSize.BoxMin.Z;

            BoxSize eighthQuadrant = fourthQuadrant.Clone();
            eighthQuadrant.BoxMax.Z = center.Z;
            eighthQuadrant.BoxMin.Z = BoxSize.BoxMin.Z;

            return new OctTreeNode[]
            {
                new OctTreeNode(firstQuadrant,this),
                new OctTreeNode(secondQuadrant,this),
                new OctTreeNode(thirdQuadrant, this),
                new OctTreeNode(fourthQuadrant, this),
                new OctTreeNode(fifthQuadrant, this),
                new OctTreeNode(sixthQuadrant, this),
                new OctTreeNode(seventhQuadrant, this),
                new OctTreeNode(eighthQuadrant, this),
            };
        }
        private Mesh GetVoxel()
        {
            double width = BoxSize.BoxMax.X - BoxSize.BoxMin.X;
            double height = BoxSize.BoxMax.Y - BoxSize.BoxMin.Y;
            double depth = BoxSize.BoxMax.Z - BoxSize.BoxMin.Z;
            return Mesh.CreateBox(width, depth, height);
        }
        private class TempHolder
        {
            public static TempHolder This = new TempHolder();
            public List<Entity> NewEntities = new List<Entity>();
            public List<Entity> OldEntities = new List<Entity>();
            public void Reset()
            {
                NewEntities.Clear(); OldEntities.Clear();
            }
        }
    }
}
