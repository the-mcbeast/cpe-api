using CPEApi.Models;
using System.Text.RegularExpressions;
namespace CPEApi.Calculation
{
    public class CalculationMethods
    {

        private readonly CPEContext _context;
        private readonly Context _sqlcontext;
        public enum parameters : ushort
        {
            VendorParam = 1,
            ProductParam = 2,
            VersionParam = 3,

        }

        public CalculationMethods(CPEContext context, Context sqlcontext)
        {
            _sqlcontext = sqlcontext;
            _context = context;
        }

        #region Matching
        #region recursive token

        public IEnumerable<Cpe>? VendorTokenRatio(string Vendor, int Threshhold)
        {
            Console.WriteLine("Vendor similarity( " + Threshhold + "): ");
            var VendorList = _sqlcontext.Cpes.AsEnumerable().Where(c => FuzzySharp.Fuzz.TokenSortRatio(c.Vendor.Replace("_", " ").ToLower(), Vendor.ToLower()) >= Threshhold);
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor: " + VendorList.First().Vendor + " FOUND");
                return VendorList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }


        public IEnumerable<Tuple<int, string>>? TopTokenRatio(string Vendor, int Threshhold, parameters param)
        {
            IEnumerable<Tuple<int, string>>? Collection;
            switch (param)
            {
                case parameters.VendorParam:

                    Collection = _sqlcontext.Cpes.AsEnumerable().Select(c => new Tuple<int, string>(
                        FuzzySharp.Fuzz.TokenSortRatio(c.Vendor.Replace("_", " ").ToLower(), Vendor.ToLower()),
                        c.Vendor)
                    ).OrderByDescending(c => c.Item1).DistinctBy(c => c.Item2).Take(Threshhold);
                    break;
              
                case parameters.ProductParam:

                    Collection = _sqlcontext.Cpes.AsEnumerable().Select(c => new Tuple<int, string>(
                        FuzzySharp.Fuzz.TokenSortRatio(c.Product.Replace("_", " ").ToLower(), Vendor.ToLower()),
                        c.Vendor)
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


        public IEnumerable<Cpe>? TitleTokenRatio(string Title, int Threshhold)
        {
            Console.WriteLine("Title similarity( " + Threshhold + "): ");
            var TitleList = _sqlcontext.Cpes.AsEnumerable().Where(c => FuzzySharp.Fuzz.TokenSortRatio(c.Title.Replace("_", " ").ToLower(), Title.ToLower()) >= Threshhold);
            if (TitleList.ToList().Any())
            {
                Console.WriteLine("Title: " + TitleList.First().Title + " FOUND");
                return TitleList;
            }
            else
            {
                Console.WriteLine(" FAILED");
                return null;
            }
        }

        public IEnumerable<Cpe>? ProductTokenRatio(IEnumerable<Cpe> List, string Product, int Threshhold)
        {
            Console.WriteLine("ProductEqualMatch similarity( " + Threshhold + "): ");
            var ProductList = List.Where(c => FuzzySharp.Fuzz.TokenSortRatio(c.Product.Replace(" ", "_").ToLower(), Product.ToLower()) >= Threshhold);
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
        public IQueryable<Cpe>? ProductEqualMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductEqualMatch:");
            var ProductList = List.Where(c => c.Product == Product.Replace(" ", "_").ToLower());
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
        public IQueryable<Cpe>? ProductPartEqualMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartEqualMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            ProductList = List.Where(c => productParts.Any(val => c.Product.Equals(val)));
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + " FOUND"); return ProductList; }
            else { Console.WriteLine(" FAILED"); return null; }
        }
        public IQueryable<Cpe>? ProductPartContainsAllMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartContainsAllMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");

            switch (productParts.Length)
            {
                case 1:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
                case 2:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    && c.Product.Contains(productParts[1]));
                    break;
                case 3:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2]));
                    break;
                case 4:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3]));
                    break;
                case 5:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3])
                   && c.Product.Contains(productParts[4]));
                    break;
                case 6:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3])
                   && c.Product.Contains(productParts[4])
                   && c.Product.Contains(productParts[5]));
                    break;
                case 7:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3])
                   && c.Product.Contains(productParts[4])
                   && c.Product.Contains(productParts[5])
                   && c.Product.Contains(productParts[6]));
                    break;
                case 8:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3])
                   && c.Product.Contains(productParts[4])
                   && c.Product.Contains(productParts[5])
                   && c.Product.Contains(productParts[6])
                   && c.Product.Contains(productParts[7]));
                    break;
                case 9:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   && c.Product.Contains(productParts[2])
                   && c.Product.Contains(productParts[3])
                   && c.Product.Contains(productParts[4])
                   && c.Product.Contains(productParts[5])
                   && c.Product.Contains(productParts[6])
                   && c.Product.Contains(productParts[7])
                   && c.Product.Contains(productParts[8])
                   );
                    break;
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + " FOUND"); return ProductList; }
            else { Console.WriteLine(" FAILED"); return null; }
        }
        public IQueryable<Cpe>? ProductPartContainsAnyMatch(IQueryable<Cpe> List, string Product)
        {
            Console.WriteLine("ProductPartContainsAnyMatch");
            IQueryable<Cpe> ProductList;
            string[] productParts = Product.ToLower().Split(" ");

            switch (productParts.Length)
            {
                case 1:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
                case 2:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    || c.Product.Contains(productParts[1]));
                    break;
                case 3:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    || c.Product.Contains(productParts[1])
                    || c.Product.Contains(productParts[2]));
                    break;
                case 4:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    || c.Product.Contains(productParts[1])
                    || c.Product.Contains(productParts[2])
                    || c.Product.Contains(productParts[3]));
                    break;
                case 5:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    || c.Product.Contains(productParts[1])
                    || c.Product.Contains(productParts[2])
                    || c.Product.Contains(productParts[3])
                    || c.Product.Contains(productParts[4]));
                    break;
                case 6:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   || c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                   || c.Product.Contains(productParts[3])
                   || c.Product.Contains(productParts[4])
                   || c.Product.Contains(productParts[5]));
                    break;
                case 7:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   || c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                   || c.Product.Contains(productParts[3])
                   || c.Product.Contains(productParts[4])
                   || c.Product.Contains(productParts[5])
                   || c.Product.Contains(productParts[6]));
                    break;
                case 8:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   || c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                   || c.Product.Contains(productParts[3])
                   || c.Product.Contains(productParts[4])
                   || c.Product.Contains(productParts[5])
                   || c.Product.Contains(productParts[6])
                   || c.Product.Contains(productParts[7]));
                    break;
                case 9:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   || c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                   || c.Product.Contains(productParts[3])
                   || c.Product.Contains(productParts[4])
                   || c.Product.Contains(productParts[5])
                   || c.Product.Contains(productParts[6])
                   || c.Product.Contains(productParts[7])
                   || c.Product.Contains(productParts[8]));
                    break;
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }

            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + " FOUND"); return ProductList; }
            else { Console.WriteLine(" FAILED"); return null; }

        }
        #endregion

        #region recursive distance
        public IEnumerable<Cpe>? recursiveLower(string Product, int comp)
        {
            Console.WriteLine(comp);
            var alt = _sqlcontext.Cpes.AsEnumerable().Where(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower()) > comp).OrderByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower())).AsEnumerable();
            if (alt.Any())
            { return alt; }
            else
            {
                if (comp > 0)
                { return recursiveLower(Product, comp - 10); }
                else
                { return null; }
            }
        }
        #endregion

        #region Version
        public IEnumerable<Cpe>? CompareVersions(IEnumerable<Cpe> List, string version2)
        {
            IEnumerable<Cpe> versionList = Enumerable.Empty<Cpe>();
            int threshhold = 0;
            while (!versionList.Any())
            { versionList = List.Where(o => CompareVersion((o.Version.Contains('.')) ? o.Version.Split('.') : new string[1] { o.Version }, version2.Contains('.') ? version2.Split('.') : new string[1] { version2 }, threshhold++)); }
            return versionList;
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

        public List<CPEItem> OrderInverseAlgorithmProduct(IEnumerable<Cpe> Query, string Product)
        {
            string[] productParts = Regex.Replace(Product, @"[^\p{L}\p{N}]+", " ").ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int[] partlength = new int[productParts.Length];
            int[] partQuantity = new int[productParts.Length];
            float[] partWeight = new float[productParts.Length];
            int ProductNumber = Query.Count();
            //subtract spaces
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
                    partWeight[index] = len + num;
                }
                else
                { partWeight[index] = 0; }
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
                GlobalQueryScore = 1,
                LocalQueryScore = 1

            }).ToList();
            foreach (CPEItem item in items)
            {
                foreach (string part in productParts)
                {
                    if (item.Product.ToLower().Contains(part))
                    {
                        int partPosition = Array.IndexOf(productParts, part);
                        item.LocalQueryScore = item.LocalQueryScore * partWeight[partPosition];


                        var b = _sqlcontext.Tfproducts.Where(x => x.Term == part.ToLower()).FirstOrDefault();
                        if (b != null)
                        {
                            item.GlobalQueryScore = item.GlobalQueryScore + b.TfIdfLogNorm;

                        }
                        //  * item.GlobalQueryScore;
                    }
                }
            }
            items = items.OrderByDescending(c => c.LocalQueryScore).ThenByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower())).ThenBy(x => Fastenshtein.Levenshtein.Distance(x.Product, Product.Replace(" ", "_").ToLower())).ToList();
            return items;
        }
        public List<CPEItem> OrderInverseAlgorithmTitle(IEnumerable<Cpe> Query, string Title)
        {
            string[] TitleParts = Regex.Replace(Title, @"[^\p{L}\p{N}]+", " ").ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int[] partlength = new int[TitleParts.Length];
            int[] partQuantity = new int[TitleParts.Length];
            float[] partWeight = new float[TitleParts.Length];
            int TitleNumber = Query.Count();
            int Titlelength = Title.Length;

            int index = 0;

            foreach (string part in TitleParts)
            {
                partlength[index] = part.Length;
                partQuantity[index] = Query.Where(c => c.Title.ToLower().Contains(part)).Count();
                if (partQuantity[index] > 0 && partlength[index] > 0)
                {
                    float num = ((float)TitleNumber / (float)partQuantity[index]);
                    float len = ((float)partlength[index] / (float)Titlelength);
                    partWeight[index] = len + num;
                }
                else
                { partWeight[index] = 0; }
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
                GlobalQueryScore = 1,
                LocalQueryScore = 1
            }).ToList();
            foreach (CPEItem item in items)
            {
                foreach (string part in TitleParts)
                {
                    if (item.Product.ToLower().Contains(part))
                    {
                        int partPosition = Array.IndexOf(TitleParts, part);
                        item.LocalQueryScore = item.LocalQueryScore * partWeight[partPosition];
                    }
                }
            }
            items = items.OrderByDescending(c => c.LocalQueryScore).ThenByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.CpeTitle.ToLower(), Title.ToLower())).ThenBy(x => Fastenshtein.Levenshtein.Distance(x.CpeTitle.ToLower(), Title.Replace(" ", "_").ToLower())).ToList();
            return items;
        }
        //IBM Tivoli Business Systems Manager
        public bool CPEItemExists(int id)
        {
            return (_context.CPEItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public void evaluateListProduct(string Product, List<CPEItem> ProductList)
        {
            Console.WriteLine(Product.Replace(" ", "_").ToLower() + "\n");
            List<Tuple<string, float>> singleProductList = new List<Tuple<string, float>>();
            foreach (CPEItem item in ProductList)
            {
                float score = (item.LocalQueryScore != null) ? (float)item.LocalQueryScore : 0;
                Tuple<string, float> pair = new Tuple<string, float>(item.Product, score);
                if (!singleProductList.Contains(pair))
                {
                    singleProductList.Add(pair);
                }
            }
            Console.WriteLine(singleProductList.Count() + "  different Products");
            int i = 0;
            foreach (var p in singleProductList)
            {
                if (i > 15) { break; }
                Console.WriteLine(p.Item1 + " Score " + p.Item2 + " [ Levenshtein " + Fastenshtein.Levenshtein.Distance(p.Item1.Replace("_", " ").ToLower(), Product.Replace("_", " ").ToLower()) + " / FuzzySharp " + FuzzySharp.Fuzz.WeightedRatio(p.Item1.Replace("_", " ").ToLower(), Product.Replace("_", " ").ToLower()) + "]");
                i++;
            }

        }







        //public string ProbableCPE(IQueryable<CPEItem> List)
        //{
        //    List.ToList();
        //    List<string> VendorList = new List<string>();
        //    List<string> ProductList = new List<string>();
        //    foreach (CPEItem item in List)
        //    {
        //        if (!VendorList.Contains(item.Vendor))
        //        {
        //            VendorList.Add(item.Vendor);
        //        }
        //        if (!ProductList.Contains(item.Product))
        //        {
        //            ProductList.Add(item.Product);
        //        }
        //    }
        //    string Vendors = string.Empty;
        //    string Products = string.Empty;
        //    foreach (string v in VendorList)
        //    {
        //        Vendors = Vendors + "/" + v;
        //    }
        //    if (ProductList.Count < 15)
        //    {
        //        foreach (string p in ProductList)
        //        {
        //            Products = Products + "/" + p;
        //        }
        //    }
        //    else { Products = "unclear"; }
        //    return "CPE : Vendor =[" + Vendors + "] Product =[" + Products + "]";

        //}
    }
}
