using CPEApi.Models;
using System.Text.RegularExpressions;
using System.Runtime;
namespace CPEApi.Calculation
{
    public class CalculationMethods
    {

        private readonly CPEContext _context;

        public CalculationMethods(CPEContext context)
        {
            _context = context;
        }


        public IQueryable<CPEItem> VendorPerfectMatch(string Vendor)
        {
            var VendorList = _context.CPEItems.Where(c => c.Vendor == Vendor.Replace(" ", "_").ToLower());
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor:" + Vendor + "Found");
                return VendorList;
            }
            else
            {
                Console.WriteLine("No Perfect Vendor Match");
                return null;
            }
        }
        public IQueryable<CPEItem> VendorContains(string Vendor)
        {
            var VendorList = _context.CPEItems.Where(c => c.Vendor.Contains(Vendor.ToLower()));
            if (VendorList.ToList().Any())
            {
                Console.WriteLine("Vendor Containing:" + Vendor + "Found");
                return VendorList;
            }
            else
            {
                Console.WriteLine("NO Vendor Found");
                return null;
            }
        }
        public IQueryable<CPEItem> ProductPerfectMatch(IQueryable<CPEItem> List, string Product)
        {
            var ProductList = List.Where(c => c.Product == Product.Replace(" ", "_").ToLower());
            if (ProductList.ToList().Any())
            {
                Console.WriteLine(ProductList.ToList().Count() + "Perfect Product Match Found" + ProbableCPE(ProductList));
                return ProductList;
            }
            else
            {
                Console.WriteLine("ProductPerfectMatch FAILED");
                return null;
            }
        }
        public IQueryable<CPEItem> ProductContainsMatch(IQueryable<CPEItem> List, string Product)
        {
            var ProductList = List.Where(c => c.Product.Contains(Product.Replace(" ", "_").ToLower()));
            if (ProductList.ToList().Any())
            {
                Console.WriteLine(ProductList.ToList().Count() + "Product Containing Match Found" + ProbableCPE(ProductList));
                return ProductList;
            }
            else
            {
                Console.WriteLine("ProductContainsMatch FAILED");
                return null;
            }
        }
        public IQueryable<CPEItem> ProductPartPerfectMatch(IQueryable<CPEItem> List, string Product)
        {
            IQueryable<CPEItem> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            switch (productParts.Length)
            {
                case 1:
                    ProductList = List.Where(c => c.Product == productParts[0]);
                    break;
                case 2:
                    ProductList = List.Where(c => c.Product == productParts[0] || c.Product == productParts[1]);
                    break;
                case 3:
                    ProductList = List.Where(c => c.Product == productParts[0] || c.Product == productParts[1] || c.Product == productParts[2]);
                    break;
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + "Products Matches for Substring Found" + ProbableCPE(ProductList)); return ProductList; }
            else { Console.WriteLine("ProductPartPerfectMatch FAILED"); return null; }
        }
        public IQueryable<CPEItem> ProductPartAllContainsMatch(IQueryable<CPEItem> List, string Product)
        {
            IQueryable<CPEItem> ProductList;
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
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + "Product Match Containing multiple Substring Found" + ProbableCPE(ProductList)); return ProductList; }
            else { Console.WriteLine("ProductPartAllContainsMatch FAILED"); return null; }
        }
        public IQueryable<CPEItem> ProductPartMultipleContainsMatch(IQueryable<CPEItem> List, string Product)
        {
            IQueryable<CPEItem> ProductList;
            string[] productParts = Product.ToLower().Split(" ");
            switch (productParts.Length)
            {
                case 2:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                    && c.Product.Contains(productParts[1]));
                    break;
                case 3:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                    && c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                    && c.Product.Contains(productParts[0]));
                    break;
                case 4:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                    && c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                    && c.Product.Contains(productParts[0]) ||
                    c.Product.Contains(productParts[0])
                   && c.Product.Contains(productParts[3])
                   || c.Product.Contains(productParts[3])
                    && c.Product.Contains(productParts[1])
                   || c.Product.Contains(productParts[2])
                    && c.Product.Contains(productParts[3]));
                    break;
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }

            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + "Product Match Containing multiple Substring Found" + ProbableCPE(ProductList)); return ProductList; }
            else { Console.WriteLine("ProductPartMultipleContainsMatch FAILED"); return null; }
        }
        public IQueryable<CPEItem> ProductPartAnyContainsMatch(IQueryable<CPEItem> List, string Product)
        {
            IQueryable<CPEItem> ProductList;
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
                default:
                    ProductList = List.Where(c => c.Product.Contains(productParts[0]));
                    break;
            }
            if (ProductList.ToList().Any())
            { Console.WriteLine(ProductList.ToList().Count() + "Product Match Containing any Substring Found" + ProbableCPE(ProductList)); return ProductList; }
            else { Console.WriteLine("ProductPartAnyContainsMatch FAILED"); return null; }

        }
        public List<CPEItem> OrderInverseAlgorithm(IQueryable<CPEItem> Query, string Product)
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
                partQuantity[index] = Query.Where(c => c.Product.Contains(part)).Count();
                if (partQuantity[index] > 0 && partlength[index] > 0)
                {
                    partWeight[index] = (ProductNumber / partQuantity[index]) * (1 + (Productlength / partlength[index]));
                }
                else
                { partWeight[index] = 0; }
                Console.WriteLine(part + " length " + partlength[index] + " quant " + partQuantity[index] + " weight "+ partWeight[index]);
                index++;
            }            
            List<CPEItem> items = Query.ToList();
            foreach (CPEItem item in items)
            {
                item.QueryScore = 0;
                foreach(string part in productParts)
                {
                    if (item.Product.Contains(part))
                    {
                        int partPosition = Array.IndexOf(productParts, part);
                        item.QueryScore = item.QueryScore + partWeight[partPosition];
                    }
                }
            }
            items = items.OrderByDescending(c => c.QueryScore).ToList();
            return items;
        }
        public IQueryable<CPEItem> recursiveLower(string Product, int comp)
        {
            Console.WriteLine(comp);
            var alt = _context.CPEItems.Where(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower()) > comp).OrderByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower()));
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
        public bool CPEItemExists(int id)
        {
            return (_context.CPEItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public void evaluateList(string Product, List<CPEItem> ProductList)
        {
            Console.WriteLine(Product.Replace(" ", "_").ToLower() + "\n");
            List<Tuple<string, float>> singleProductList = new List<Tuple<string, float>>();
            foreach (CPEItem item in ProductList)
            {
                float score = (item.QueryScore != null) ? (float)item.QueryScore : 0;
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
                if(i > 15) { break; }
                                Console.WriteLine(p.Item1 + " Levenshtein " + Fastenshtein.Levenshtein.Distance(p.Item1, Product.Replace(" ", "_").ToLower()) + " Score " + p.Item2);
                i++;
            }

        }
        public string ProbableCPE(IQueryable<CPEItem> List)
        {
            List.ToList();
            List<string> VendorList = new List<string>();
            List<string> ProductList = new List<string>();
            foreach (CPEItem item in List)
            {
                if (!VendorList.Contains(item.Vendor))
                {
                    VendorList.Add(item.Vendor);
                }
                if (!ProductList.Contains(item.Product))
                {
                    ProductList.Add(item.Product);
                }
            }
            string Vendors = string.Empty;
            string Products = string.Empty;
            foreach (string v in VendorList)
            {
                Vendors = Vendors + "/" + v;
            }
            if (ProductList.Count < 15)
            {
                foreach (string p in ProductList)
                {
                    Products = Products + "/" + p;
                }
            }
            else { Products = "unclear"; }
            return "CPE : Vendor =[" + Vendors + "] Product =[" + Products + "]";

        }
    }
}
