using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
namespace DataFileManager
{
    [Serializable]
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        XmlSerializer ks = new XmlSerializer(typeof(TKey));
        XmlSerializer vs = new XmlSerializer(typeof(TValue));

        #region 构造函数  

        public XmlDictionary()
            : base()
        {
        }

        public XmlDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public XmlDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public XmlDictionary(int capacity)
            : base(capacity)
        {
        }

        public XmlDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected XmlDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion 构造函数

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");

                reader.ReadStartElement("Key");
                TKey k = (TKey)ks.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("Value");
                TValue v = (TValue)vs.Deserialize(reader);
                reader.ReadEndElement();

                Add(k, v);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            
            foreach (TKey key in Keys)
            {
                writer.WriteStartElement("Item");
                writer.WriteStartElement("Key");
                ks.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                TValue value = this[key];
                vs.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }
    }
}
