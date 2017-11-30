﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Mono.Documentation.Updater.Frameworks
{
    class FrameworkEntry
    {
        SortedSet<FrameworkTypeEntry> types = new SortedSet<FrameworkTypeEntry> ();

        List<FrameworkEntry> allframeworks;
        int index = 0;

        public FrameworkEntry (List<FrameworkEntry> frameworks)
        {
            allframeworks = frameworks;
            if (allframeworks == null)
                allframeworks = new List<FrameworkEntry> (0);

            index = allframeworks.Count;
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }

        public IEnumerable<DocumentationImporter> Importers { get; set; }

        public ISet<FrameworkTypeEntry> Types { get { return this.types; } }
        Dictionary<string, FrameworkTypeEntry> typeMap = new Dictionary<string, FrameworkTypeEntry> ();

        public FrameworkTypeEntry FindTypeEntry (FrameworkTypeEntry type) 
        {
            return FindTypeEntry (type.Name);    
        }

        /// <param name="name">The value from <see cref="FrameworkTypeEntry.Name"/>.</param>
        public FrameworkTypeEntry FindTypeEntry (string name) {
            FrameworkTypeEntry entry;
            typeMap.TryGetValue (name, out entry);
            return entry;
        }

		public IEnumerable<FrameworkEntry> Frameworks { get { return this.allframeworks; } }

		public static readonly FrameworkEntry Empty = new EmptyFrameworkEntry () { Name = "Empty" };

		public virtual FrameworkTypeEntry ProcessType (TypeDefinition type)
		{
            FrameworkTypeEntry entry;

            if (!typeMap.TryGetValue (type.FullName, out entry)) {
				var docid = DocCommentId.GetDocCommentId (type);
				entry = new FrameworkTypeEntry (this) { Id = docid, Name = type.FullName, Namespace = type.Namespace };
				types.Add (entry);

                typeMap.Add (entry.Name, entry);
			}
			return entry;
		}

		public override string ToString () => this.Name;

		class EmptyFrameworkEntry : FrameworkEntry
		{
			public EmptyFrameworkEntry () : base (null) { }
			public override FrameworkTypeEntry ProcessType (TypeDefinition type) { return FrameworkTypeEntry.Empty; }
		}
	}
}
