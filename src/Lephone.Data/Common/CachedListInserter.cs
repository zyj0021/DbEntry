﻿using Lephone.Data.Caching;

namespace Lephone.Data.Common
{
    public class CachedListInserter : IProcessor
    {
        private readonly IProcessor _processor;

        public CachedListInserter(IProcessor processor)
        {
            this._processor = processor;
        }

		public bool Process(object obj)
		{
            var success = _processor.Process(obj);
            if (success)
            {
                CacheProvider.Instance[KeyGenerator.Instance[obj]] = ModelContext.CloneObject(obj);
            }
		    return success;
		}
    }
}
