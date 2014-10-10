//
// BrickTypeInfo.cs
//
// Author:
//       Evan Reidland <er@evanreidland.com>
//
// Copyright (c) 2014 Evan Reidland
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

namespace Spacebrick
{
    public class NameIDManager
    {
        public const short MaximumID = 0x3FF; //1023

        private Dictionary<string, ushort> _idsByName = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<ushort, string> _namesByID = new Dictionary<ushort, string>();

        private ushort CreateID(string name)
        {
            ushort id;
            string otherName;
            if (!_idsByName.TryGetValue(name, out id))
            {
                //Probably slow, but it might not matter because ids should only created/loaded. when the game initializes.
                //IDs should save out to JSON.
                for (id = 1; id <= MaximumID; id++)
                {
                    if (!_namesByID.TryGetValue(id, out otherName))
                    {
                        _namesByID[id] = name;
                        _idsByName[name] = id;
                        return id;
                    }
                }
            }

            return id;
        }

        public ushort RegisterName(string name) { return CreateID(name); }

        public ushort GetID(string name)
        {
            ushort id;
            _idsByName.TryGetValue(name, out id);
            return id;
        }

        public string GetName(ushort id)
        {
            string name;
            _namesByID.TryGetValue(id, out name);
            return name;
        }
    }

    public class BlockTypeInfo
    {
        private static NameIDRegistry<BlockTypeInfo> _blockTypeRegistry = new NameIDRegistry<BlockTypeInfo>();
        private static NameIDManager _nameIDManager = new NameIDManager();

        public static BlockTypeInfo GetTypeInfo(string name) { return _blockTypeRegistry.GetItem(name); }
        public static BlockTypeInfo GetTypeInfo(int id) { return _blockTypeRegistry.GetItem(id); }

        private static void Register(BlockTypeInfo info)
        {
            _blockTypeRegistry.RegisterItem(info, info.Name, info.ID);
        }

        public string Name { get; private set; }
        public ushort ID { get; private set; }
        public bool HasDirection { get; private set; }

        public BlockTypeInfo(string name, bool hasDirection)
        {
            Name = name;
            if (!string.IsNullOrEmpty(name))
                ID = _nameIDManager.RegisterName(name);

            Register(this);
        }
    }
}

