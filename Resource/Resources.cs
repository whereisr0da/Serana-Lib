/**
 * Serana - Copyright (c) 2018 - 2020 r0da [r0da@protonmail.ch]
 *
 * This work is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-nd/4.0/ or send a letter to
 * Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
 *
 * By using Serana, you agree to the above license and its terms.
 *
 *      Attribution - You must give appropriate credit, provide a link to the license and indicate if changes were
 *                    made. You must do so in any reasonable manner, but not in any way that suggests the licensor
 *                    endorses you or your use.
 *
 *   Non-Commercial - You may not use the material (Serana) for commercial purposes.
 *
 *   No-Derivatives - If you remix, transform, or build upon the material (Serana), you may not distribute the
 *                    modified material. You are, however, allowed to submit the modified works back to the original
 *                    Serana project in attempt to have it added to the original project.
 *
 * You may not apply legal terms or technological measures that legally restrict others
 * from doing anything the license permits.
 *
 * No warranties are given.
 */

using System;
using System.Collections.Generic;

using Serana.Engine.Headers;
using Serana.Engine.Headers.Types;
using Serana.Engine.Resource.Types;
using Serana.Engine.Section;
using Serana.Engine.Streams;

namespace Serana.Engine.Resource
{
    public class Resources
    {
        private Reader reader;

        private Header header;

        private Sections sections;

        public int resourceBaseAddress;

        public List<ResourceDirectoryTable> resourceTables;

        public SectionEntry resourceSection;

        private List<Entry> entries;

        private bool isInMemory = false;

        /// <summary>
        /// Create Resource collector from file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="header"></param>
        public Resources(Reader reader, Header header, Sections sections)
        {
            this.reader = reader;
            this.header = header;
            this.sections = sections;

            this.resourceTables = new List<ResourceDirectoryTable>();
            this.entries = new List<Entry>();

            this.resourceSection = sections.getResourceSection();

            if (this.resourceSection == null)
                throw new Exception("Fail to handle resources");

            this.resourceBaseAddress = this.resourceSection.header.pointerToRawData.getValue();

            int baseOffset = this.resourceBaseAddress;

            // handle the root of the tree
            resourceTables.Add(new ResourceDirectoryTable(this, this.reader, this.entries, ref baseOffset));

            // read the next entry
            resourceTables[0].readEntries(ref baseOffset, true);
        }

        /// <summary>
        /// Get the first node of resource tree
        /// </summary>
        /// <returns>ResourceDirectoryTable of the node</returns>
        public ResourceDirectoryTable getRootNode()
        {
            return resourceTables[0];
        }

        /// <summary>
        /// Get sub node for a resource type
        /// </summary>
        /// <param name="type">Resource type</param>
        /// <returns>A list of directory entry for a resource type</returns>
        public List<ResourceDirectoryEntry> getNodesFromResourceType(ResourceTypes type)
        {
            List<ResourceDirectoryEntry> nodes = new List<ResourceDirectoryEntry>();

            ResourceDirectoryEntry startNode = getEntryFromResourceType(type);

            if (startNode.resourceTables.Count <= 0)
                return null;

            // in reality, after first node, there is only one table
            foreach (ResourceDirectoryEntry item in startNode.resourceTables[0].resourceEntries)
            {
                nodes.Add(item);
            }

            return nodes;
        }

        /// <summary>
        /// Get the data node from its resource type and id
        /// </summary>
        /// <param name="type">Resource type</param>
        /// <param name="id">Resource id</param>
        /// <returns>Data entry corresponding to arguments</returns>
        public ResourceDataEntry getResourceDataFromId(ResourceTypes type, int id)
        {
            List<ResourceDirectoryEntry> nodes = getNodesFromResourceType(type);

            if (nodes.Count <= 0)
                return null;

            ResourceDirectoryEntry selectedNode = null;

            foreach (ResourceDirectoryEntry node in nodes)
            {
                if (node.directoryId == id)
                    selectedNode = node;
            }

            if (selectedNode == null)
                return null;

            ResourceDirectoryEntry dataDirectory = null;

            // in reality, after first node, there is only one table
            foreach (ResourceDirectoryEntry item in selectedNode.resourceTables[0].resourceEntries)
            {
                // end node (not documented ?)
                if (item.directoryId == 1033)
                    dataDirectory = item;
            }

            if (dataDirectory == null)
                return null;

            return dataDirectory.dataEntry;
        }

        /// <summary>
        /// Get the entry from a resource type
        /// </summary>
        /// <param name="type">The resource type</param>
        /// <returns>The directory entry correponding to the resource type</returns>
        public ResourceDirectoryEntry getEntryFromResourceType(ResourceTypes type)
        {
            foreach (var item in getRootNode().resourceEntries)
            {
                if (item.firstNode)
                {
                    if (item.directoryType == type)
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Indicate if a resource type is in the tree
        /// </summary>
        /// <param name="type">The resource type</param>
        /// <returns>True is the type is present</returns>
        public bool isResourceTypePresent(ResourceTypes type)
        {
            foreach (var item in getRootNode().resourceEntries)
            {
                if(item.firstNode)
                {
                    if (item.directoryType == type)
                        return true;
                }
            }

            return false;
        }
    }
}
