using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public interface IVerifiable
    {
        void Validate();
    }

    [DataContract()]
    public class BaseRequest : IVerifiable
    {
        [DataMember()]
        public Guid ClientId { get; set; }

        public override string ToString()
        {
            return this.ClientId.ToString();
        }

        public virtual void Validate()
        {
            if (this.ClientId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("ClientId is null or empty");
            }
        }
    }

    [DataContract]
    public class SimpleObjectRequest<T> : BaseRequest where T : IVerifiable
    {
        [DataMember]
        public T Data { get; set; }

        public override void Validate()
        {
            this.Validate(false);
        }

        public virtual void Validate(bool canDataBeNull)
        {
            base.Validate();

            if (this.Data == null)
            {
                if (!canDataBeNull)
                {
                    throw new Pic4PicArgumentException("Input data is null");
                }
            }
            else
            {
                this.Data.Validate();
            }
        }

        public override string ToString()
        {
            if (this.Data == null)
            {
                return String.Empty;
            }
            else
            {
                return this.Data.ToString();
            }
        }
    }

    [DataContract]
    public class SimpleStructRequest<T> : BaseRequest where T : struct
    {
        [DataMember]
        public T Data { get; set; }

        public override void Validate()
        {
            base.Validate();

            if(this.Data.Equals(default(T)))
            {
                throw new Pic4PicArgumentException("Input data is null or empty");
            }
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }
    }

    [DataContract]
    public class SimpleStringRequest : BaseRequest
    {
        [DataMember]
        public String Data { get; set; }

        public override void Validate()
        {
            this.Validate(false);
        }

        public void Validate(bool canDataBeNullOrEmpty)
        {
            base.Validate();

            if (String.IsNullOrWhiteSpace(this.Data))
            {
                if (!canDataBeNullOrEmpty)
                {
                    throw new Pic4PicArgumentException("Input data is null or empty");
                }
            }
        }

        public override string ToString()
        {
            return this.Data;
        }
    }

    [DataContract]
    public class SimpleObjectListRequest<T> : BaseRequest where T : IVerifiable
    {
        [DataMember]
        public List<T> Items { get { return this.items; } }
        private List<T> items = new List<T>();

        public override void Validate()
        {
            base.Validate();

            if (this.Items == null || this.Items.Count == 0)
            {
                throw new Pic4PicArgumentException("Input list is null or empty");
            }

            foreach (T item in this.items)
            {
                item.Validate();
            }
        }
        public override string ToString()
        {
            return this.items.Count.ToString() + " items";
        }
    }

    [DataContract]
    public class SimpleStructListRequest<T> : BaseRequest where T : struct
    {
        [DataMember]
        public List<T> Items { get { return this.items; } }
        private List<T> items = new List<T>();

        public override void Validate()
        {
            base.Validate();

            if (this.Items == null || this.Items.Count == 0)
            {
                throw new Pic4PicArgumentException("Input list is null or empty");
            }

            foreach (T item in this.items)
            {
                if (item.Equals(default(T)))
                {
                    throw new Pic4PicArgumentException("Invalid item in the input list");
                }
            }
        }

        public override string ToString()
        {
            return this.items.Count.ToString() + " items";
        }
    }
}
