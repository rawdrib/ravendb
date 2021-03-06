using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Serialization;
using Raven.Client.Util;
using System.Linq;

namespace Raven.Client.Document
{
	public class DocumentConvention
	{
		public DocumentConvention()
		{
			FindIdentityProperty = q => q.Name == "Id";
			FindTypeTagName = t => DefaultTypeTagName(t);
			IdentityPartsSeparator = "/";
		    JsonContractResolver = new DefaultContractResolver(shareCache: true);
		    MaxNumberOfRequestsPerSession = 30;
		}

		public string IdentityPartsSeparator { get; set; }

		public int MaxNumberOfRequestsPerSession { get; set; }

		public static string GenerateDocumentKeyUsingIdentity(DocumentConvention conventions, object entity)
		{
			return conventions.FindTypeTagName(entity.GetType()).ToLowerInvariant() + "/";
		}

		public static string DefaultTypeTagName(Type t)
		{
			if(t.IsGenericType)
			{
				var name = t.GetGenericTypeDefinition().Name;
				if(name.Contains("`"))
				{
					name = name.Substring(0, name.IndexOf("`"));
				}
				var sb = new StringBuilder(Inflector.Pluralize(name));
				foreach (var argument in t.GetGenericArguments())
				{
					sb.Append("Of")
						.Append(DefaultTypeTagName(argument));
				}
				return sb.ToString();
			}
			return Inflector.Pluralize(t.Name);
		}

		public string GetTypeTagName(Type type)
		{
			return FindTypeTagName(type) ?? DefaultTypeTagName(type);
		}

		public string GenerateDocumentKey(object entity)
		{
			return DocumentKeyGenerator(entity);
		}

		public PropertyInfo GetIdentityProperty(Type type)
		{
			return type.GetProperties().FirstOrDefault(FindIdentityProperty);
		}

        public IContractResolver JsonContractResolver { get; set; }

		public Func<Type, string> FindTypeTagName { get; set; }
		public Func<PropertyInfo, bool> FindIdentityProperty { get; set; }

		public Func<object, string> DocumentKeyGenerator { get; set; }
	}
}