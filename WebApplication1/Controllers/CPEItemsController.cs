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
        private readonly CPEContext _context;
        private readonly Context _sqlcontext;
        public CalculationMethods Calc;
        public CPEItemsController(CPEContext context
           , Context sqlcontext
            )
        {
            _context = context;
            _sqlcontext = sqlcontext;
            Calc = new CalculationMethods(_context, _sqlcontext);
        }
        #region old
        //}
        //[HttpGet("ProdTermFreq")]
        //public async Task<ActionResult<string>> ProductTermFrequency()
        //{
        //    if (_sqlcontext.Cpes == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        var prod = _sqlcontext.Cpes.OrderBy(x => x.Product).GroupBy(x => x.Product).Select(x => x.First().Product);
        //        var terms = new List<Tfproduct>();
        //        char[] delimiterChars = { ' ', ',', '.', ':', '\t','_', '\'', '/', ':','\\' , '%', '!',  '"', '@', '#',  '&', '[', '$',  '?',   '|', '(',  ')', '>', ']' , '+', '-', '*' , '<', '=' , '^',  };
        //        var tmp = new List<string>();                        
        //        foreach (var item in prod)                           
        //        {                                                    


        //            var b = item.ToLower().Split(delimiterChars);    
        //            foreach (var a in b)                             
        //            {                                                 
        //                if (a.Length > 0)                              
        //                { tmp.Add(a); }
        //            }
        //            //count 203699

        //        }
        //        foreach (var term in tmp)
        //        {
        //            if (term.Length > 0)
        //            {
        //                if (terms.Any(o => o.Term == term))
        //                {
        //                    terms.First(o => o.Term == term).RawCount++;
        //                }
        //                else
        //                {
        //                    terms.Add(new Tfproduct
        //                    {
        //                        Term = term,
        //                        RawCount = 1
        //                    });
        //                }
        //            }
        //        }
        //        _sqlcontext.AddRange(terms);
        //        _sqlcontext.SaveChanges();
        //        return Ok();
        //    }

        //}


        //[HttpGet("VendorTermFreq")]
        //public async Task<ActionResult<string>> VendorTermFrequency()
        //{
        //    if (_sqlcontext.Cpes == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        var prod = _sqlcontext.Cpes.OrderBy(x => x.Vendor).GroupBy(x => x.Vendor).Select(x => x.First().Vendor);
        //        var terms = new List<Tfvendor>();
        //        char[] delimiterChars = { ' ', ',', '.', ':', '\t', '_', '\'', '/', ':', '\\', '%', '!', '"', '@', '#', '&', '[', '$', '?', '|', '(', ')', '>', ']', '+', '-', '*', '<', '=', '^', };

        //        var tmp = new List<string>();
        //        foreach (var item in prod)
        //        {

        //            var b = item.ToLower().Split(delimiterChars);
        //            foreach (var a in b)
        //            {
        //                if (a.Length > 0)
        //                { tmp.Add(a); }
        //            }
        //            //count 26060

        //        }
        //        foreach (var term in tmp)
        //        {
        //            if (term.Length > 0)
        //            {
        //                if (terms.Any(o => o.Term == term))
        //                {
        //                    terms.First(o => o.Term == term).RawCount++;
        //                }
        //                else
        //                {
        //                    terms.Add(new Tfvendor
        //                    {
        //                        Term = term,
        //                        RawCount = 1
        //                    });
        //                }
        //            }
        //        }
        //        _sqlcontext.AddRange(terms);
        //        _sqlcontext.SaveChanges();
        //        return Ok();
        //    }

        //}


        //[HttpGet("TitleTermFreq")]
        //public async Task<ActionResult<string>> TitleTermFrequency()
        //{
        //    if (_sqlcontext.Cpes == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        var prod = _sqlcontext.Cpes.OrderBy(x => x.Title).GroupBy(x => x.Title).Select(x => x.First().Title);
        //        var terms = new List<Tftitle>();
        //        char[] delimiterChars = { ' ', ',', '.', ':', '\t', '_', '\'', '/', ':', '\\', '%', '!', '"', '@', '#', '&', '[', '$', '?', '|', '(', ')', '>', ']', '+', '-', '*', '<', '=', '^', };

        //        var tmp = new List<string>();
        //        foreach (var item in prod)
        //        {

        //            var b = item.ToLower().Split(delimiterChars);
        //            foreach (var a in b)
        //            {
        //                if (a.Length > 0)
        //                { tmp.Add(a); }
        //            }
        //            //count 203699

        //        }
        //        foreach (var term in tmp)
        //        {
        //            if (term.Length > 0)
        //            {
        //                if (terms.Any(o => o.Term == term))
        //                {
        //                    terms.First(o => o.Term == term).RawCount++;
        //                }
        //                else
        //                {
        //                    terms.Add(new Tftitle
        //                    {
        //                        Term = term,
        //                        RawCount = 1
        //                    });
        //                }
        //            }
        //        }
        //        _sqlcontext.AddRange(terms);
        //        _sqlcontext.SaveChanges();
        //        return Ok();
        //    }

        //}
        #endregion

        //  api/CPEItems/uploadDictionary/
        [HttpGet("uploadDictionary/")]
        public async Task<ActionResult<string>> UpLoadDictionary()
        {
            XDocument xdoc = XDocument.Load(@"C:\Users\m.schwaegerl\Desktop\BA MSL\official-cpe-dictionary_v2.3.xml");
            string prefix = "{http://cpe.mitre.org/dictionary/2.0}";
            var prefix23 = "{http://scap.nist.gov/schema/cpe-extension/2.3}";
            if (xdoc.Root != null)
            {
                XElement xroot = xdoc.Root;
                var test = xroot.Descendants().Where(s => s.Name == (prefix + "cpe-item")).Select(o =>
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
                _sqlcontext.AddRange(test);
                _sqlcontext.SaveChanges();

                return "success";
            }
            else { return NotFound(); }
        }

        [HttpGet("FindByParam")]
        public ActionResult<IEnumerable<CPEItem>> FindCPEItems(string? Part, string? Vendor, string Product, string? Version)
        {
            if (_sqlcontext.Cpes == null)
            {
                return NotFound();
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
                        { //Correction Attempt here

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
            if (ProductList == null)
            {
                ProductList = Calc.ProductContainsMatch(VendorList, Product);
                if (ProductList == null)
                {
                    ProductList = Calc.ProductPartEqualMatch(VendorList, Product);
                    if (ProductList == null)
                    {
                        ProductList = Calc.ProductPartEqualMatch(VendorList, ProductNaked);
                        if (ProductList == null)
                        {
                            ProductList = Calc.ProductPartContainsAllMatch(VendorList, Product);
                            if (ProductList == null)
                            {
                                ProductList = Calc.ProductPartContainsAllMatch(VendorList, ProductNaked);
                                if (ProductList == null)
                                {
                                    ProductList = Calc.ProductPartContainsAnyMatch(VendorList, ProductNaked);
                                    if (ProductList == null)
                                    {
                                        ProductList = Calc.ProductPartContainsAnyMatch(VendorList, Product);
                                        if (ProductList == null)
                                        {
                                            return NotFound();
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

            IEnumerable<Cpe>? VersionList;
            List<CPEItem> ItemList;
            if (Version != null)
            {
                VersionList = Calc.CompareVersions(ProductList, Version);
                ItemList = Calc.OrderInverseAlgorithmProduct(VersionList, Product);
            }
            else
            {
               ItemList = Calc.OrderInverseAlgorithmProduct(ProductList, Product);
            }
            Calc.evaluateListProduct(Product, ItemList);
            return ItemList;
        }


        [HttpGet("FindByParamToken")]
        public ActionResult<IEnumerable<CPEItem>> FindCPEItemsToken(string? Part, string? Vendor, string Product, string? Version)
        {
            IEnumerable<Tuple< int, string>> vendorTuple;
            if (_sqlcontext.Cpes == null)
            {
                return NotFound();
            }
            IEnumerable<Cpe>? VendorList;

            int vthreshhold = 25;
            vendorTuple = Calc.TopTokenRatio(Vendor, vthreshhold, CalculationMethods.parameters.VendorParam);
            //if (Vendor != null)
            //{
               
            //    //while (VendorList == null)
            //    //{
            //    //    vthreshhold -= 5;
            //    //    VendorList = Calc.VendorTokenRatio(Vendor, vthreshhold);
            //    //}
            //    //VendorList = Calc.VendorTokenRatio(Vendor, vthreshhold - 10);
            //}
            //else
            //{
            //    Console.WriteLine("NO Vendor Found");
            //    VendorList = _sqlcontext.Cpes;
            //}
            foreach (var b in vendorTuple)
            {
                Console.WriteLine("Vendor similarity( " + b.Item2 + b.Item1 + "): ");
            }
            var vendorArray = vendorTuple.Select(o => o.Item2);
            VendorList = _sqlcontext.Cpes.Where(o => vendorArray.Contains(o.Vendor));
            if (Part != null) { VendorList = VendorList.Where(s => s.Part == Part); }
            Product = Product.ToLower();
            IEnumerable<Cpe>? ProductList;
            string ProductNaked = Regex.Replace(Product, @"[^\p{L}\p{N}]+", " ");
            int pthreshhold = 0;
            ProductList = Calc.ProductTokenRatio(VendorList, Product, pthreshhold);
            while (ProductList == null)
            {
                pthreshhold -= 5;
                ProductList = Calc.ProductTokenRatio(VendorList, Product, pthreshhold);
            }
            //ProductList = Calc.ProductTokenRatio(VendorList, Product, pthreshhold - 10);
            

            IEnumerable<Cpe>? VersionList;
            VersionList = Calc.CompareVersions(ProductList, Version);
            List<CPEItem> ItemList = Calc.OrderInverseAlgorithmProduct(VersionList, Product);
            Calc.evaluateListProduct(Product, ItemList);
            return ItemList;
        }
        [HttpGet("TitleTioken")]
        public ActionResult<IEnumerable<CPEItem>> TitleToken(string? Title)
        {
            if (_sqlcontext.Cpes == null)
            {
                return NotFound();
            }
            IEnumerable<Cpe>? TitleList;
            if (Title != null)
            {
                int vthreshhold = 100;
                TitleList = Calc.TitleTokenRatio(Title, vthreshhold);
                while (TitleList == null)
                {
                    vthreshhold -= 5;
                    TitleList = Calc.TitleTokenRatio(Title, vthreshhold);
                }
                TitleList = Calc.TitleTokenRatio(Title, vthreshhold - 10);
            }
            else
            {
                Console.WriteLine("NO Vendor Found");
                TitleList = _sqlcontext.Cpes;
            }
            List<CPEItem> ItemList = Calc.OrderInverseAlgorithmTitle(TitleList, Title);
            Calc.evaluateListProduct(Title, ItemList);
            return ItemList;
        }
        //[HttpGet("FindLeven/{Product}")]
        //public ActionResult<IEnumerable<Cpe>> FindLeven(string? Product)
        //{
        //    if (_sqlcontext.Cpes == null || Product == null || Product == string.Empty)
        //    {
        //        return NotFound();
        //    }
        //    var res = Calc.recursiveLower(Product, 100);
        //    if (res != null)
        //    {
        //        res.OrderByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower()));
        //        Calc.evaluateList(Product, res.Select(x => new CPEItem
        //        {
        //            CpeName = x.Name,
        //            CpeTitle = x.Title,
        //            Part = x.Part,
        //            Vendor = x.Vendor,
        //            Edition = x.Edition,
        //            Target_hw = x.TargetHw,
        //            Target_sw = x.TargetSw,
        //            Language = x.Language,
        //            Id = x.Id,
        //            Other = x.Other,
        //            Product = x.Product,
        //            Sw_edition = x.SwEdition,
        //            Update = x.Update,
        //            Version = x.Version,
        //            QueryScore = 1
        //        }).ToList());
        //        return res.ToList();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}
        // GET: api/CPEItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cpe>> GetCPEItem(int id)
        {
            if (_sqlcontext.Cpes == null)
            {
                return NotFound();
            }
            var cPEItem = await _sqlcontext.Cpes.FindAsync(id);

            if (cPEItem == null)
            {
                return NotFound();
            }

            return cPEItem;
        }


    }
}
