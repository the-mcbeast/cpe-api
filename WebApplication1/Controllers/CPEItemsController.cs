using CPEApi.Calculation;
using CPEApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace CPEApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CPEItemsController : ControllerBase
    {
        private readonly Context _sqlcontext;
        public CalculationMethods Calc;
        public CPEItemsController(Context sqlcontext)
        {
            _sqlcontext = sqlcontext;
            Calc = new CalculationMethods(_sqlcontext);
        }

        [HttpGet("uploadDictionary/")]
        public async Task<ActionResult<string>> UpLoadDictionary()
        {
            XDocument xdoc = XDocument.Load(@"C:\Users\m.schwaegerl\Desktop\BA MSL\official-cpe-dictionary_v2.3.xml");
            string prefix = "{http://cpe.mitre.org/dictionary/2.0}";
            var prefix23 = "{http://scap.nist.gov/schema/cpe-extension/2.3}";
            if (xdoc.Root != null)
            {
                XElement xroot = xdoc.Root;
                var cpes = xroot.Descendants().Where(s => s.Name == (prefix + "cpe-item")).Select(o =>
                {
                    var parts = o.Descendants().
                                       Where(s => s.Name == (prefix23 + "cpe23-item")).
                                       FirstOrDefault().
                                       Attribute("name").Value.Split(':');
                    return new Cpe()
                    {
                        Name = o.Attribute("name").Value,
                        Title = o.Descendants().
                                       Where(s => s.Name == (prefix + "title")).
                                       FirstOrDefault().Value,
                        Part = parts[2],
                        Vendor = parts[3],
                        Product = parts[4],
                        Version = parts[5],
                        Update = parts[6],
                        Edition = parts[7],
                        Language = parts[8],
                        SwEdition = parts[9],
                        TargetSw = parts[10],
                        TargetHw = parts[11],
                        Other = parts[12]
                    };
                });
                _sqlcontext.AddRange(cpes);
                _sqlcontext.SaveChanges();

                return "success";
            }
            else { return NotFound(); }
        }

        [HttpGet("FindByParamSet")]
        public ActionResult FindCPEItemsSet(string? Part, string? Vendor, string Product, string? Version)
        {
            string Method = "SET";
            Part = (Part != null) ? Part.ToLower().Replace("_", " ") : null;
            Vendor = (Vendor != null) ? Vendor.ToLower().Replace("_", " ") : null;
            Product = Product.ToLower().Replace("_", " ");
            Version = (Version != null) ? Version.ToLower().Replace("_", " ") : null;
            if (!_sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).Any())
            {
                _sqlcontext.Add(new Anfragen { Part = Part, Vendor = Vendor, Product = Product, Version = Version, Created = DateTime.Now });
                _sqlcontext.SaveChanges();
            }
            else
            {
                if (_sqlcontext.Antworten2s.Where(o => o.Typ == _sqlcontext.Typs.Where(x => x.Name == Method).FirstOrDefault().Id && o.AnfrageId == _sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).FirstOrDefault().Id).Any())
                {
                    return Ok();
                }
            }

            Console.WriteLine("START" + DateTime.Now.ToString("mm-ss"));
            if (_sqlcontext.Cpes == null)
            {
                return Ok();
            }
            IQueryable<Cpe>? VendorList;
            if (Vendor != null)
            {
                VendorList = Calc.VendorEqualMatch(Vendor);
                if (VendorList == null)
                {
                    VendorList = Calc.VendorContains(Vendor);
                    if (VendorList == null)
                    {
                        VendorList = Calc.ContainsVendor(Vendor);
                        if (VendorList == null)
                        {
                            VendorList = _sqlcontext.Cpes;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("NO Vendor Found");
                VendorList = _sqlcontext.Cpes;
            }
            if (Part != null) { VendorList = VendorList.Where(s => s.Part == Part); }
            Product = Product.ToLower();
            IQueryable<Cpe>? ProductList;
            string ProductNaked = Regex.Replace(Product, @"[^\p{L}\p{N}]+", " ");
            ProductList = Calc.ProductEqualMatch(VendorList, Product);

            List<CPEItem>? VersionList = new List<CPEItem>();
            List<CPEItem> ItemList = new List<CPEItem>();
            if (ProductList == null)
            {
                ProductList = Calc.ProductContainsMatch(VendorList, Product);
                if (ProductList == null)
                {

                    ProductList = Calc.ProductPartContainsMultipleMatch(VendorList, Product);
                    if (ProductList == null)
                    {
                        ProductList = Calc.ProductPartContainsMultipleMatch(VendorList, ProductNaked);
                        if (ProductList == null)
                        {
                            ProductList = Calc.ProductPartEqualMatch(VendorList, Product);
                            if (ProductList == null)
                            {
                                ProductList = Calc.ProductPartEqualMatch(VendorList, ProductNaked);
                                if (ProductList == null)
                                {
                                    ProductList = Calc.ProductPartContainsAnyMatch(VendorList, ProductNaked);
                                    if (ProductList == null)
                                    {
                                        ProductList = Calc.ProductPartContainsAnyMatch(VendorList, Product);
                                        if (ProductList == null)
                                        {
                                            return SaveToDB(ItemList, Part, Vendor, Product, Version, Method);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Version != null && ProductList.Any())
            {

                VersionList = Calc.OrderInverseAlgorithmProduct(ProductList, Product);
                ItemList = Calc.CompareVersions(VersionList, Version);
            }
            else
            {
                ItemList = Calc.OrderInverseAlgorithmProduct(ProductList, Product);
            }
            Console.WriteLine("END" + DateTime.Now.ToString("mm-ss"));
            return SaveToDB(ItemList, Part, Vendor, Product, Version, Method);
        }


        [HttpGet("FindByParamToken")]
        public ActionResult FindCPEItemsToken(string? Part, string? Vendor, string Product, string? Version, int vthreshhold = 25, int pthreshhold = 50)
        {
            string Method = "LEVEN";
            Part = (Part != null) ? Part.ToLower().Replace("_", " ") : null;
            Vendor = (Vendor != null) ? Vendor.ToLower().Replace("_", " ") : null;
            Product = Product.ToLower().Replace("_", " ");
            Version = (Version != null) ? Version.ToLower().Replace("_", " ") : null;
            if (!_sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).Any())
            {
                _sqlcontext.Add(new Anfragen { Part = Part, Vendor = Vendor, Product = Product, Version = Version, Created = DateTime.Now });
                _sqlcontext.SaveChanges();
            }
            else
            {
                if (_sqlcontext.Antworten2s.Where(o => o.Typ == _sqlcontext.Typs.Where(x => x.Name == Method).FirstOrDefault().Id && o.AnfrageId == _sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).FirstOrDefault().Id).Any())
                {
                    return Ok();
                }
            }
            Console.WriteLine("START" + DateTime.Now.ToString("mm-ss"));
            IEnumerable<Tuple<int, string>> vendorTuple;
            if (_sqlcontext.Cpes == null)
            {
                return Ok();
            }
            IEnumerable<Cpe>? VendorList;
            IEnumerable<Cpe>? ProductList;
            IEnumerable<Tuple<int, string>> productTuple;
            if (Vendor != null)
            {
                vendorTuple = Calc.TopTokenRatio(Vendor, vthreshhold, CalculationMethods.parameters.VendorParam);

                foreach (var b in vendorTuple)
                {
                    Console.WriteLine("Vendor similarity( " + b.Item2 + " -- " + b.Item1 + "): ");
                }

                var vendorArray = vendorTuple.Select(o => o.Item2).ToArray();

                productTuple = Calc.TopTokenRatio(Product, pthreshhold, CalculationMethods.parameters.ProductParam);

                var ProductArray = productTuple.Select(o => o.Item2).ToArray();
                ProductList = _sqlcontext.Cpes.Where(o => ProductArray.Contains(o.Product) && vendorArray.Contains(o.Vendor)).ToList();
            }
            else
            {

                productTuple = Calc.TopTokenRatio(Product, pthreshhold, CalculationMethods.parameters.ProductParam);

                var ProductArray = productTuple.Select(o => o.Item2).ToArray();
                ProductList = _sqlcontext.Cpes.Where(o => ProductArray.Contains(o.Product)).ToList();
            }
            foreach (var b in productTuple)
            {
                Console.WriteLine("Product similarity( " + b.Item2 + " -- " + b.Item1 + "): ");
            }
            if (Part != null) { ProductList = ProductList.Where(s => s.Part == Part); }
            List<CPEItem> VersionList;
            List<CPEItem> ItemList;
            if (Version != null && ProductList.Any())
            {
                VersionList = Calc.OrderInverseAlgorithmProduct(ProductList, Product);
                ItemList = Calc.CompareVersions(VersionList, Version);
            }
            else
            {
                ItemList = Calc.OrderInverseAlgorithmProduct(ProductList, Product);
            }
            Console.WriteLine("END" + DateTime.Now.ToString("mm-ss"));
            return SaveToDB(ItemList, Part, Vendor, Product, Version, Method);
        }


        [HttpGet("FindByParamIterate")]
        public ActionResult FindCPEItemsIterate(string? Part, string? Vendor, string Product, string? Version)
        {
            string Method = "CONTROL";
            var Part1 = (Part != null) ? Part.ToLower().Replace("_", " ") : null;
            var Vendor1 = (Vendor != null) ? Vendor.ToLower().Replace("_", " ") : null;
            var Product1 = Product.ToLower().Replace("_", " ");
            var Version1 = (Version != null) ? Version.ToLower().Replace("_", " ") : null;
            if (!_sqlcontext.Anfragens.Where(x => x.Part == Part1 && x.Vendor == Vendor1 && x.Product == Product1 && x.Version == Version1).Any())
            {
                _sqlcontext.Add(new Anfragen { Part = Part1, Vendor = Vendor1, Product = Product1, Version = Version1, Created = DateTime.Now });
                _sqlcontext.SaveChanges();
            }
            else
            {
                if (_sqlcontext.Antworten2s.Where(o => o.Typ == _sqlcontext.Typs.Where(x => x.Name == Method).FirstOrDefault().Id && o.AnfrageId == _sqlcontext.Anfragens.Where(x => x.Part == Part1 && x.Vendor == Vendor1 && x.Product == Product1 && x.Version == Version1).FirstOrDefault().Id).Any())
                {
                    return Ok();
                }
            }
            Console.WriteLine("START" + DateTime.Now.ToString("mm-ss"));
            Part = (Part != null) ? (Part.Contains(" ")) ? Part.ToLower().Replace(" ", "_") : Part.ToLower() : null;
            Vendor = (Vendor != null) ? (Vendor.Contains(" ")) ? Vendor.ToLower().Replace(" ", "_") : Vendor.ToLower() : null;
            Product = (Product.Contains(" ")) ? Product.ToLower().Replace(" ", "_") : Product.ToLower();
            Version = (Version != null) ? (Version.Contains(" ")) ? Version.ToLower().Replace(" ", "_") : Version.ToLower() : null;
            var disjointProduct = new Tuple<CalculationMethods.Element, CalculationMethods.ComparisonResult>(CalculationMethods.Element.Product, CalculationMethods.ComparisonResult.Disjoint);
            var disjointVendor = new Tuple<CalculationMethods.Element, CalculationMethods.ComparisonResult>(CalculationMethods.Element.Vendor, CalculationMethods.ComparisonResult.Disjoint);
            var CPEs = _sqlcontext.Cpes.ToList().Where(x => !Calc.Compare_WFNs(x, new Cpe { Part = Part ?? "ANY", Vendor = Vendor ?? "ANY", Product = Product, Version = Version ?? "ANY" }).Contains(disjointProduct) && !Calc.Compare_WFNs(x, new Cpe { Part = Part ?? "ANY", Vendor = Vendor ?? "ANY", Product = Product, Version = Version ?? "ANY" }).Contains(disjointVendor));

            List<CPEItem> ItemList = Calc.OrderInverseAlgorithmProduct(CPEs.AsEnumerable(), Product);
            if (Version != null && ItemList.Any())
            { ItemList = Calc.CompareVersions(ItemList.AsEnumerable(), Version); }      
            Console.WriteLine("END" + DateTime.Now.ToString("mm-ss"));
            return SaveToDB(ItemList, Part1, Vendor1, Product1, Version1, Method);

        }
        private ActionResult SaveToDB(List<CPEItem> ItemList, string? Part, string? Vendor, string Product, string? Version, string Method)
        {
            int counter = 0;
            foreach (var item in ItemList)
            {
                _sqlcontext.Antworten2s.AddAsync(new Antworten2
                {
                    AnfrageId = _sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).FirstOrDefault().Id,
                    Typ = _sqlcontext.Typs.Where(x => x.Name == Method).FirstOrDefault().Id,
                    Cpeid = item.Id,
                    ResultNr = counter,
                    LocalScore = item.LocalQueryScore,
                    GlobalScore = item.GlobalQueryScore,
                    Created = DateTime.Now
                });
                counter++;
                if (counter > 5)
                {
                    _sqlcontext.SaveChanges();
                    return Ok();
                }
            }
            _sqlcontext.Antworten2s.AddAsync(new Antworten2
            {
                AnfrageId = _sqlcontext.Anfragens.Where(x => x.Part == Part && x.Vendor == Vendor && x.Product == Product && x.Version == Version).FirstOrDefault().Id,
                Typ = _sqlcontext.Typs.Where(x => x.Name == Method).FirstOrDefault().Id,
                Cpeid = null,
                ResultNr = counter,
                LocalScore = null,
                GlobalScore = null,
                Created = DateTime.Now
            });
            _sqlcontext.SaveChanges();
            return Ok();
        }
    }
}
