using System;
using System.Collections.Generic;
using System.Configuration;

namespace Tarro.Configuration
{
    [ConfigurationCollection(typeof(ApplicationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]

    internal class ApplicationCollection : ConfigurationElementCollection, IEnumerable<ApplicationElement>
    {
        public ApplicationElement this[int index]
        {
            get { return (ApplicationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(ApplicationElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplicationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApplicationElement)element).Name;
        }

        public void Remove(ApplicationElement serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(String name)
        {
            BaseRemove(name);

        }

        public new IEnumerator<ApplicationElement> GetEnumerator()
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as ApplicationElement;
            }
        }
    }
}