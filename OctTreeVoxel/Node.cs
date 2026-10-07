using devDept.Eyeshot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volvex
{
    public class Node<INode>
    {
        private INode _currentEntity;
        private readonly IList<Node<INode>> _childrens=new List<Node<INode>>();
        public Node(INode entity)
        {
            _currentEntity = entity;
        }
        public void AddChild(Node<INode> childNode)
        {
            _childrens.Add(childNode);
        }
    }
}
