//-----------------------------------------------------------------------
// <copyright company="your company" file="XmlHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  builder  name: arson
//  build    time: 2020-03-10
//  function desc: xml helper class
//  history  list:
//           2020-03-10 arison build the xml helper class!
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NET2CommonHelper
{
    /// <summary>
    /// xml helper class
    /// </summary>
    public class XmlHelper : IDisposable
    {
        FileInfo _fileInfo = null;
        XmlDocument _xmlDoc = null;
        private XmlNode _root = null;

        public XmlDocument XmlDoc => _xmlDoc;

        public XmlNode RootNode => _root;

        public string DefaultNamespaceURI => RootNode.NamespaceURI;

        public string DefaultPrefix => RootNode.Prefix;

        #region init xml helper class
        /// <summary>
        /// load xml from special file path
        /// </summary>
        /// <param name="filePath">xml file path</param>
        public XmlHelper(string filePath) : this()
        {
            _fileInfo = new FileInfo(filePath);
            _xmlDoc.Load(filePath);
            _root = _xmlDoc.DocumentElement;
        }

        public XmlHelper()
        {
            _xmlDoc = new XmlDocument();
        }

        /// <summary>
        /// create root node
        /// </summary>
        /// <param name="name">root node name</param>
        /// <param name="prefix">prefix of root node</param>
        /// <param name="nameSpace">namespace of root node</param>
        public void CreateRootElement(string name,
                                        string version = "1.0",
                                        string encoding = "utf-8",
                                        string prefix = "",
                                        string nameSpace = "")
        {
            //add xml declaration,<?xml version="1.0" encoding="utf-8"?>
            XmlDeclaration xmldecl;
            xmldecl = _xmlDoc.CreateXmlDeclaration(version, encoding, null);
            _xmlDoc.AppendChild(xmldecl);
            //add root node（declaration node just only has a child node）
            var root = _xmlDoc.CreateElement(prefix, name, nameSpace);
            _xmlDoc.AppendChild(root);
            _root = root;
        }

        /// <summary>
        /// load xml document from xml string
        /// </summary>
        /// <param name="xml">xml string</param>
        public void LoadFromXmlString(string xml)
        {
            _xmlDoc.LoadXml(xml);
            _root = _xmlDoc.DocumentElement;
        }

        /// <summary>
        /// load xml document from xml file path
        /// </summary>
        /// <param name="filePath">xml file path</param>
        public void LoadFromFile(string filePath)
        {
            _xmlDoc.Load(filePath);
            _root = _xmlDoc.DocumentElement;
            _fileInfo = new FileInfo(filePath);
        }
        #endregion

        /// <summary>
        /// create node
        /// </summary>
        /// <param name="name">node name</param>
        /// <param name="useDefaultNamespace">whether or not use the default namespace</param>
        /// <returns>xml node</returns>
        public XmlNode CreateNode(string name, bool useDefaultNamespace = false)
        {
            XmlNode node = _xmlDoc.CreateElement(useDefaultNamespace ? DefaultPrefix : null,
                                                    name,
                                                    useDefaultNamespace ? DefaultNamespaceURI : string.Empty);
            return node;
        }

        /// <summary>
        /// create node
        /// </summary>
        /// <param name="name">node name</param>
        /// <param name="prefix">prefix of the node</param>
        /// <param name="namespaceURI">namespace URI</param>
        /// <param name="nodeType">node type enum</param>
        /// <returns>xml node</returns>
        public XmlNode CreateNode(string name, string prefix, string namespaceURI = "", XmlNodeType nodeType = XmlNodeType.Element)
        {
            XmlNode node = _xmlDoc.CreateNode(nodeType, prefix, name, namespaceURI);
            return node;
        }

        /// <summary>
        /// create new node
        /// </summary>
        /// <param name="name">node name</param>
        public XmlNode AppendNode(string name)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>(0);
            return AppendNode(name, attributes);
        }

        /// <summary>
        /// create new node
        /// </summary>
        /// <param name="name">node name</param>
        /// <param name="attributes">node's attributes dictionary( key/value pairs)</param>
        public XmlNode AppendNode(string name, Dictionary<string, string> attributes)
        {
            XmlNode node = _xmlDoc.CreateElement(name);
            CreateAttributes(attributes, node);
            _root.AppendChild(node);
            return node;
        }

        /// <summary>
        /// create new node for existed node's name
        /// </summary>
        /// <param name="existedNodeName">existed node's name</param>
        /// <param name="nodeName">new node's name</param>

        public XmlNode AppendNodeToExistedNodeName(string existedNodeName, string nodeName)
        {
            Dictionary<string, string> defaults = new Dictionary<string, string>(0);
            return AppendNodeToExistedNodeName(existedNodeName, nodeName, defaults);
        }

        /// <summary>
        /// create new node for existed node's name
        /// </summary>
        /// <param name="exitNodeName">existed node's name</param>
        /// <param name="nodeName">new node's name</param>
        /// <param name="attributes">new node's attributes dictionary( key/value pairs)</param>
        public XmlNode AppendNodeToExistedNodeName(string exitNodeName, string nodeName, Dictionary<string, string> attributes)
        {
            XmlNode exitNode = _xmlDoc[exitNodeName];
            XmlNode node = _xmlDoc.CreateElement(nodeName);
            CreateAttributes(attributes, node);
            exitNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// create new node for existed node
        /// </summary>
        /// <param name="existedNode">existed node</param>
        /// <param name="nodeName">new node's name</param>
        public XmlNode AppendNodeToExistedNode(XmlNode existedNode, string nodeName)
        {
            Dictionary<string, string> defaults = new Dictionary<string, string>(0);
            return AppendNodeToExistedNode(existedNode, nodeName, defaults);
        }

        /// <summary>
        /// create new node for existed node
        /// </summary>
        /// <param name="exitNode">existed node</param>
        /// <param name="nodeName">new node's name</param>
        /// <param name="attributes">new node's attributes dictionary( key/value pairs)</param>
        public XmlNode AppendNodeToExistedNode(XmlNode existedNode, string nodeName, Dictionary<string, string> attributes)
        {
            XmlNode node = _xmlDoc.CreateElement(nodeName);
            CreateAttributes(attributes, node);
            existedNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// create node's attributes set
        /// </summary>
        /// <param name="attributes">node's attributes dictionary( key/value pairs)</param>
        /// <param name="node">node</param>
        private void CreateAttributes(Dictionary<string, string> attributes, XmlNode node)
        {
            foreach (var key in attributes.Keys)
            {
                var attribute = _xmlDoc.CreateAttribute(key);
                attribute.Value = attributes[key];
                node.Attributes.Append(attribute);
            }
        }

        /// <summary>
        /// save current xml document to default file
        /// </summary>
        public void Save()
        {
            _xmlDoc.Save(_fileInfo.FullName);
        }

        /// <summary>
        /// save as another file path
        /// </summary>
        /// <param name="filePath">another file path</param>
        /// <param name="openFileAfterSaved">after saved whether or not open this file!</param>
        public void SaveAs(string filePath, bool openFileAfterSaved = false)
        {
            _fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(_fileInfo.Directory.FullName))
            {
                Directory.CreateDirectory(_fileInfo.Directory.FullName);
            }
            this.Save();
            if (openFileAfterSaved)
            {
                System.Diagnostics.Process.Start(filePath);
            }
        }

        /// <summary>
        /// file name
        /// </summary>
        public string Name => _fileInfo.Name;

        /// <summary>
        /// free resources
        /// </summary>
        public void Dispose()
        {
            _xmlDoc = null;
            _fileInfo = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get xml node
        /// </summary>
        /// <param name="match">match mode</param>
        /// <returns>xml node</returns>
        public XmlNode GetXmlNode(Predicate<XmlNode> match)
        {
            return GetNodeByPredicate(_root, match);
        }


        /// <summary>
        /// get node from indicated node
        /// </summary>
        /// <param name="node">indicated node</param>
        /// <param name="match">match mode</param>
        /// <returns>xml node</returns>
        private XmlNode GetNodeByPredicate(XmlNode node, Predicate<XmlNode> match)
        {
            XmlNode result = node;
            if (match.Invoke(node)) return node;
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    result = GetNodeByPredicate(child, match);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        /// <summary>
        /// output the xml document's xml string
        /// </summary>
        /// <returns>xml string</returns>
        public override string ToString()
        {
            return XmlDoc.OuterXml;
        }

    }
}
