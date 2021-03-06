﻿using Raven.Database;
using Raven.Database.Plugins;

namespace Raven.Tests.Triggers
{
	public class CascadeDeleteTrigger : AbstractDeleteTrigger 
	{
        public override VetoResult AllowDelete(string key, TransactionInformation transactionInformation)
		{
			return VetoResult.Allowed;
		}

		public override void OnDelete(string key, TransactionInformation transactionInformation)
		{
			var document = Database.Get(key, null);
			if (document == null)
				return;
            Database.Delete(document.Metadata.Value<string>("Cascade-Delete"), null, null);
		}

		public override void AfterCommit(string key)
		{
		}
	}
}