﻿namespace GameFreeText
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class SimpleTextBranch
    {
        public string BranchName;
        public List<SimpleTextLeaf> Leaves = new List<SimpleTextLeaf>();

        internal void LoadFromXmlNode(XmlNode rootNode)
        {
            foreach (XmlNode node in rootNode.ChildNodes)
            {
                SimpleTextLeaf item = new SimpleTextLeaf {
                    Text = node.Attributes.GetNamedItem("Text").Value
                };
                item.TextColor.PackedValue = uint.Parse(node.Attributes.GetNamedItem("Color").Value);
                this.Leaves.Add(item);
            }
        }
    }
}

