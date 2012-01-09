﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace EppLib.Entities
{
    public abstract class EppCommand<T> : EppBase<T> where T : EppResponse
    {
        private readonly string nspace;
        protected readonly string namespaceUri;

        /// <summary>
        /// Lenght is 6 - 16, ascii chars
        /// </summary>
        public string Password;

        public readonly IList<EppExtension> Extensions = new List<EppExtension>();
        private static readonly string ClTrid = Guid.NewGuid().ToString();

        protected EppCommand(string nspace, string namespaceUri)
        {
            this.nspace = nspace;
            this.namespaceUri = namespaceUri;
        }

        protected EppCommand()
        {
        }

        private void SetCommonAttributes(XmlElement command)
        {
            command.SetAttribute("xmlns:" + nspace, namespaceUri);
        }

        protected XmlElement BuildCommandElement(XmlDocument doc, string qualifiedName/*, IEnumerable<CiraExtension> mExtensions = null*/, string query = null)
        {
            var commandRootElement = GetCommandRootElement(doc);

            var command = GetCommand(doc, qualifiedName, commandRootElement, query);

            if (Extensions != null)
            {
                PrepareExtensionElement(doc, commandRootElement, Extensions);
            }

            if (!String.IsNullOrWhiteSpace(Password))
            {
                var authInfo = AddXmlElement(doc, command, "domain:authInfo", null, namespaceUri);

                AddXmlElement(doc, authInfo, "domain:pw", Password, namespaceUri);
            }

            return command;
        }

        private XmlElement GetCommand(XmlDocument doc, string qualifiedName, XmlElement commandRootElement, string query = null)
        {
            var elem = CreateElement(doc, qualifiedName);

            if(query !=null)
            {
                elem.SetAttribute("op", query);
            }

            var command = CreateElement(doc, nspace + ":" + qualifiedName);

            SetCommonAttributes(command);

            elem.AppendChild(command);

            commandRootElement.AppendChild(elem);

            return command;
        }

        protected static XmlElement GetCommandRootElement(XmlDocument doc)
        {
            var root = CreateDocRoot(doc);

            doc.AppendChild(root);

            var command = CreateElement(doc, "command");

            root.AppendChild(command);

            var clTRIDElement = CreateElement(doc, "clTRID");

            clTRIDElement.InnerText = ClTrid;

            command.AppendChild(clTRIDElement);
            
            return command;
        }

        
    }
}