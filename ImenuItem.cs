using Microsoft.VisualStudio.Extensibility.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Imenu
{
    [DataContract]
    internal class ImenuItem : NotifyPropertyChangedObject
    {

        /// <summary>
        /// リスト表示名
        /// </summary>
        private string _name = string.Empty;
        [DataMember]
        public string Name
        {
            get => _name;
            set => SetProperty(ref this._name, value);
        }

        /// <summary>
        /// ジャンプ先情報
        /// </summary>
        public int Line { get; set; } = 0;
    }
}
