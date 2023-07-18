using CPEApi.Models;
using System.Text.RegularExpressions;
namespace CPEApi.Calculation
{
    public class CalculationMethods
    {

        private readonly Context _sqlcontext;
        public enum parameters : ushort
        {
            VendorParam = 1,
            ProductParam = 2,
            VersionParam = 3,

        }

        public enum ComparisonResult
        {
            Disjoint,
            Equal,
            Subset,
            Superset
        }

        public enum Element
        {
            Part,
            Product,
            Version,
            Vendor
        }


        public CalculationMethods(Context sqlcontext)
        {
            _sqlcontext = sqlcontext;
        }

        #region Matching
        #region tokenDistance

        public IEnumerable<Tuple<int, string>>? TopTokenRatio(string Token, int Threshhold, parameters param)
        {
            IEnumerable<Tuple<int, string>>? Collection;
            switch (param)
            {
                case parameters.VendorParam:

                    Collection = _sqlcontext.Cpes.AsEnumerable().Select(c => new Tuple<int, string>(
                        FuzzySharp.Fuzz.TokenSortRatio(c.Vendor.Replace("_", " ").ToLower(), Token.ToLower().Replace("_", " ")),
                        c.Vendor)
                    ).OrderByDescending(c => c.Item1).DistinctBy(c => c.Item2).Take(Threshhold);
                    break;

                case parameters.ProductParam:

                    Collection = _sqlcontext.Cpes.AsEnumerable().Select(c => new Tuple<int, string>(
                        FuzzySharp.Fuzz.TokenSortRatio(c.Product.Replace("_", " ").ToLower(), Token.Replace("_", " ").ToLower()),
                        c.Product)
                    ).ToList().OrderByDescending(c => c.Item1).DistinctBy(c => c.Item2);
                    Collection = Collection.OrderByDescending(c => c.Item1).DistinctBy(c => c.Item2).Take(Threshhold);
                    break;

                case parameters.VersionParam:

                    Collection = _sqlcontext.Cpes.AsEnumerable().Select(c => new Tuple<int, string>(
                        FuzzySharp.Fuzz.TokenSortRatio(c.Version.Replace("_", " ").ToLower(), Token.ToLower()),
                        c.Version)
                    ).OrderByDescending(c => c.Item1).DistinctBy(c => c.Item2).Take(Threshhold);
                    break;
                default:
                    return null;
            }


            if (Collection.ToList().Any())
            {
                return Collection;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }



        #endregion

        #region mengenschrumpfen
        public IQueryable<Cpe>? VendorEqualMatch(string Vendor)
        {
            Console.WriteLine("VendorEqualsMatch: ");
            var VendorList = _sqlcontext.Cpes.Where(c => c.Vendor == Vendor.Replace(" ", "_").ToLower());
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor: " + VendorList.ToList().First() + " FOUND");
                return VendorList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }
        public IQueryable<Cpe>? VendorContains(string Vendor)
        {
            Console.WriteLine("VendorContains: ");
            var VendorList = _sqlcontext.Cpes.Where(c => c.Vendor.Contains(Vendor.Replace(" ", "_").ToLower()));
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor: " + VendorList.ToList().First() + " FOUND");
                return VendorList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }

        public IQueryable<Cpe>? ContainsVendor(string Vendor)
        {
            Console.WriteLine("VendorContains: ");
            var VendorList = _sqlcontext.Cpes.Where(c => Vendor.Contains(c.Vendor.Replace("_", string.Empty).ToLower()));
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor: " + VendorList.ToList().First() + " FOUND");
                return VendorList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }
        /// <summary>
        /// Matches the Product given to CPEs equal to the Product
        /// </summary>
        /// <param name="List"></param>
        /// <param name="Product"></param>
        /// <returns></returns>
        public IQueryable<Cpe>? ProductEqualMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductEqualMatch:");
            var ProductList = List.Where(c => c.Product == Product.Replace(" ", "_").ToLower());
            if (ProductList.ToList().Any())
            {
                Console.WriteLine(ProductList.ToList().Count + " FOUND ");
                return ProductList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }
        /// <summary>
        /// Matches the Product given to CPEs containing the entire Product
        /// </summary>
        /// <param name="List"></param>
        /// <param name="Product"></param>
        /// <returns></returns>
        public IQueryable<Cpe>? ProductContainsMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductContainsMatch:");
            var ProductList = List.Where(c => c.Product.Contains(Product.Replace(" ", "_").ToLower()));
            if (ProductList.ToList().Any())
            {
                Console.WriteLine(ProductList.ToList().Count() + " FOUND ");
                return ProductList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }

        /// <summary>
        /// Matches the Product given to CPEs with any product  with one of the tokens as title
        /// </summary>
        /// <param name="List"></param>
        /// <param name="Product"></param>
        /// <returns></returns>
        public IQueryable<Cpe>? ProductPartEqualMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartEqualMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            ProductList = List.Where(c => productParts.Any(val => c.Product.Equals(val)));
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count + " FOUND"); return ProductList; }
            else { Console.WriteLine(" FAILED"); return null; }
        }


        public IQueryable<Cpe>? ProductPartContainsMultipleMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartContainsMultipleMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            var query = _sqlcontext.Cpes.ToList().Select(x => new
            {
                x,
                count = productParts.Count(o => x.Product.Contains(o))
            }).ToList();
            int max = query.Max(p => p.count);
            ProductList = query.Where(p => p.count.Equals(max)).Select(n => n.x).AsQueryable();
            if (ProductList.ToList().Any() && max >1)
            {
                Console.WriteLine(ProductList.ToList().Count + " FOUND with Threshhold: " + max);
                return ProductList;
            }
            else { Console.WriteLine(" FAILED"); return null; }
        }

        /// <summary>
        /// Matches the Product given to CPEs containing any tokens in its Product field
        /// </summary>
        /// <param name="List"></param>
        /// <param name="Product"></param>
        /// <returns></returns>
        public IQueryable<Cpe>? ProductPartContainsAnyMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartContainsAnyMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            Predicate<Tuple<Cpe, string[]>> predicate = containsAny;
            ProductList = List.ToList().Where(c => predicate(new Tuple<Cpe, string[]>(c, productParts))).AsQueryable();

            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + " FOUND"); return ProductList; }
            else { Console.WriteLine(" FAILED"); return null; }

        }
        public static bool containsAny(Tuple<Cpe, string[]> productParts)
        {
            foreach (string productPart in productParts.Item2)
            {
                if (productParts.Item1.Product.Contains(productPart))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region NIST

        public List<Tuple<Element, ComparisonResult>> Compare_WFNs(Cpe source, Cpe target)
        {
            List<Tuple<Element, ComparisonResult>> resultList = new List<Tuple<Element, ComparisonResult>>();

            resultList.Add(new Tuple<Element, ComparisonResult>(Element.Part, compare(source.Part, target.Part)));
            resultList.Add(new Tuple<Element, ComparisonResult>(Element.Vendor, compare(source.Vendor, target.Vendor)));
            resultList.Add(new Tuple<Element, ComparisonResult>(Element.Product, compare(source.Product, target.Product)));
            resultList.Add(new Tuple<Element, ComparisonResult>(Element.Version, compare(source.Version, target.Version)));

            return resultList;
        }

        private ComparisonResult compare(string source, string target)
        {
            if (!string.IsNullOrEmpty(source))
                source = source.ToLower();

            if (!string.IsNullOrEmpty(target))
                target = target.ToLower();

            if (ContainsWildcards(target))
                return ComparisonResult.Disjoint;

            if (source == target)
                return ComparisonResult.Equal;

            if (source == "any")
                return ComparisonResult.Superset;

            if (target == "any")
                return ComparisonResult.Subset;

            if (source == "na" || target == "na")
                return ComparisonResult.Disjoint;

            return compareStrings(source, target);
        }

        private ComparisonResult compareStrings(string source, string target)
        {
            int start = 0;
            int end = source.Length;
            int begins = 0;
            int ends = 0;

            if (source.StartsWith("*"))
            {
                start = 1;
                begins = -1;
            }
            else
            {
                while (start < source.Length && source[start] == '?')
                {
                    start++;
                    begins++;
                }
            }

            if (source.EndsWith("*") && isEvenWildcards(source, end - 1))
            {
                end--;
                ends = -1;
            }
            else
            {
                while (end > 0 && source[end - 1] == '?' && isEvenWildcards(source, end - 1))
                {
                    end--;
                    ends++;
                }
            }

            source = source.Substring(start, end - start);
            int index = -1;
            int leftover = target.Length;

            while (leftover > 0)
            {
                index = target.IndexOf(source, index + 1);

                if (index == -1)
                    break;

                int escapes = countEscapeCharacters(target, 0, index);

                if (index > 0 && begins != -1 && begins < (index - escapes))
                    break;

                escapes = countEscapeCharacters(target, index + 1, target.Length);
                leftover = target.Length - index - escapes - source.Length;

                if (leftover > 0 && ends != -1 && leftover > ends)
                    continue;

                return ComparisonResult.Superset;
            }

            return ComparisonResult.Disjoint;
        }

        private int countEscapeCharacters(string str, int start, int end)
        {
            int result = 0;
            bool active = false;

            for (int i = 0; i < end; i++)
            {
                if (str[i] == '\\')
                    active = !active;

                if (active && i >= start)
                    result++;
            }

            return result;
        }

        private bool isEvenWildcards(string str, int idx)
        {
            int result = 0;

            while (idx > 0 && str[idx - 1] == '\\')
            {
                idx--;
                result++;
            }

            return result % 2 == 0;
        }

        private bool ContainsWildcards(string str)
        {
            return str.Contains("?") || str.Contains("*");
        }

        #endregion

        #region Version
        public List<CPEItem>? CompareVersions(IEnumerable<CPEItem> List, string version2)
        {
            List<CPEItem> versionList = new List<CPEItem>();
            if (version2.Contains('.'))
            {
                int threshhold = 0;
                while (!versionList.Any())
                { versionList = List.Where(o => CompareVersion((o.Version.Contains('.')) ? o.Version.Split('.') : new string[1] { o.Version }, version2.Contains('.') ? version2.Split('.') : new string[1] { version2 }, threshhold++)).ToList(); }   
            }
            else
            {
                versionList =  List.Select(c => new Tuple<float, CPEItem>(
                      FuzzySharp.Fuzz.PartialTokenSetRatio(c.Version.Replace("_", " ").ToLower(), version2.ToLower().Replace("_", " ")),
                      c)
                  ).OrderByDescending(o => o.Item1).Select(o => o.Item2).ToList();               
            }
            return versionList.GroupBy(o => new { o.Product, o.Version, o.Vendor }).Select(x => x.First()).ToList();
        }
        bool CompareVersion(string[] components1, string[] components2, int parts)
        {
            int length = Math.Min(components1.Length, components2.Length);
            var ignore = 0;
            for (int i = 0; i < length - parts; i++)
            {
                if (int.TryParse(components1[i], out ignore))
                {
                    int value1 = i < components1.Length ? int.Parse(components1[i]) : 0;
                    if (int.TryParse(components2[i], out ignore))
                    {
                        int value2 = i < components2.Length ? int.Parse(components2[i]) : 0;
                        int comparison = value1.CompareTo(value2);
                        if (comparison != 0)
                        {
                            return false;
                        }
                    }
                    else { return false; }
                }
                else { return false; }
            }

            return true;
        }

        #endregion

        #endregion Matching

        #region Evaluation
        public List<CPEItem> OrderInverseAlgorithmProduct(IEnumerable<Cpe> Query, string Product)
        {
            string[] productParts = Regex.Replace(Product, @"[^\p{L}\p{N}]+", " ").ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int[] partlength = new int[productParts.Length];
            int[] partQuantity = new int[productParts.Length];
            float[] partWeight = new float[productParts.Length];
            int ProductNumber = Query.Count();
            int Productlength = Product.Length;
            int index = 0;
            foreach (string part in productParts)
            {
                partlength[index] = part.Length;
                partQuantity[index] = Query.Where(c => c.Product.ToLower().Contains(part)).Count();
                if (partQuantity[index] > 0 && partlength[index] > 0)
                {
                    float num = ((float)ProductNumber / (float)partQuantity[index]);
                    float len = ((float)partlength[index] / (float)Productlength);
                    partWeight[index] = (len * num) / ProductNumber;
                }
                else
                { partWeight[index] = 1; }
                Console.WriteLine(part + " length " + partlength[index] + " quant " + partQuantity[index] + " weight " + partWeight[index]);
                index++;
            }
            List<CPEItem> items = Query.AsEnumerable().Select(x => new CPEItem
            {
                CpeName = x.Name,
                CpeTitle = x.Title,
                Part = x.Part,
                Vendor = x.Vendor,
                Edition = x.Edition,
                Target_hw = x.TargetHw,
                Target_sw = x.TargetSw,
                Language = x.Language,
                Id = x.Id,
                Other = x.Other,
                Product = x.Product,
                Sw_edition = x.SwEdition,
                Update = x.Update,
                Version = x.Version,
                GlobalQueryScore = 0,
                LocalQueryScore = 1

            }).ToList();
            foreach (CPEItem item in items)
            {
                int divider = 0;
                foreach (string part in productParts)
                {
                    if (item.Product.ToLower().Contains(part))
                    {

                        int partPosition = Array.IndexOf(productParts, part);
                        item.LocalQueryScore = item.LocalQueryScore * partWeight[partPosition];


                        var b = _sqlcontext.Tfproducts.Where(x => x.Term == part.ToLower()).FirstOrDefault();

                        if (b != null)
                        {
                            divider += 1;
                            item.GlobalQueryScore +=  1/b.LogNormalized;

                        }
                    }
                }
                if (divider == 0)
                {
                    item.GlobalQueryScore = 0;
                }
                else
                {
                    item.GlobalQueryScore /= divider;
                }

            }
            items = items.OrderBy(c => c.LocalQueryScore).ThenByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower())).ThenBy(x => Fastenshtein.Levenshtein.Distance(x.Product, Product.Replace(" ", "_").ToLower())).ToList();
            return items;
        }
    }
    #endregion
}
