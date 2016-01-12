using System;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public enum DinerType
    {
        Undefined,
        Occasional,
        Frequent,
        Gourmet,
    }

    public enum CustomerType
    {
        Undefined,
        New,
        Occasional,
        Frequent,
    }

    public class Diner
    {
        public static DinerType ConvertToDinerType(int checkinCount)
        {
            if (checkinCount <= 0)
            {
                return DinerType.Undefined;
            }
            else if (checkinCount < 15)
            {
                return DinerType.Occasional;
            }
            else if (checkinCount < 30)
            {
                return DinerType.Frequent;
            }
            else 
            {
                return DinerType.Gourmet;
            }
        }

        public static CustomerType ConvertToCustomerType(int checkinCount)
        {
            if (checkinCount <= 0)
            {
                return CustomerType.Undefined;
            }
            else if (checkinCount == 1)
            {
                return CustomerType.New;
            }
            else if (checkinCount < 5)
            {
                return CustomerType.Occasional;
            }
            else
            {
                return CustomerType.Frequent;
            }
        }

        public static string ToString(DinerType dinerType)
        {
            return ToString(dinerType, false);
        }

        public static string ToString(DinerType dinerType, bool includeA)
        {
            if (dinerType == DinerType.Occasional)
            {
                if (includeA)
                {
                    return "an Occasional diner";
                }
                else
                {
                    return "Eats out occasionally";
                }
            }
            else if (dinerType == DinerType.Frequent)
            {
                if (includeA)
                {
                    return "a Frequent diner";
                }
                else
                {
                    return "Eats out frequently";
                }                
            }
            else if (dinerType == DinerType.Gourmet)
            {
                if (includeA)
                {
                    return "a Gourmet";
                }
                else
                {
                    return "Eats out very frequently";
                }
            }

            return String.Empty;
        }

        public static string ToString(CustomerType customerType)
        {
            if (customerType == CustomerType.New)
            {
                return "(new with us)";
            }
            else if (customerType == CustomerType.Occasional)
            {
                return "(occasionally with us)";
            }
            else if (customerType == CustomerType.Frequent)
            {
                return "(frequently with us)";
            }

            return String.Empty;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Status Status { get; set; }

        public Diner() { }

        public Diner(string flatText)
        {
            if (!String.IsNullOrWhiteSpace(flatText))
            {
                flatText = flatText.Trim();
                string[] tokens = flatText.Split(':');
                if (tokens.Length == 2)
                {
                    tokens[0] = tokens[0].Trim();
                    tokens[1] = tokens[1].Trim();
                    if (!String.IsNullOrWhiteSpace(tokens[0]) && !String.IsNullOrWhiteSpace(tokens[1]))
                    {
                        Guid id;
                        if (Guid.TryParse(tokens[0], out id))
                        {
                            if (id != Guid.Empty)
                            {
                                this.Id = id;
                                this.UserName = tokens[1];
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Id, this.UserName);
        }
    }
}
