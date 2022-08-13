using CPEApi.Calculation;
using CPEApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public CalculationMethods Calc;
        public CPEItemsController(CPEContext context)
        {
            _context = context;
            Calc = new CalculationMethods(_context);
        }



        //  api/CPEItems/loadDictionary/
        [HttpGet("loadDictionary/")]
        public async Task<ActionResult<string>> loadDictionary()
        {


            XDocument xdoc = XDocument.Load(@"C:\Users\m.schwaegerl\Desktop\BA MSL\official-cpe-dictionary_v2.3.xml");
            var prefix = "{http://cpe.mitre.org/dictionary/2.0}";
            var prefix23 = "{http://scap.nist.gov/schema/cpe-extension/2.3}";
            XElement xroot = xdoc.Root;
            var cpes = from el in xroot.Descendants().
                       Where(s => s.Name == (prefix + "cpe-item"))
                       select new
                       {
                           Name = el.Attribute("name").Value,
                           Title = el.Descendants().
                                   Where(s => s.Name == (prefix + "title")).
                                   FirstOrDefault().Value,
                           WFN = el.Descendants().
                                   Where(s => s.Name == (prefix23 + "cpe23-item")).
                                   FirstOrDefault().
                                   Attribute("name").Value,
                       };
            foreach (var element in cpes)
            {
                string[] parts = element.WFN.Split(':');
                CPEItem cPEItem = new CPEItem()
                {
                    CpeName = element.Name,
                    CpeTitle = element.Title,
                    valid = true,
                    Part = parts[2],
                    Vendor = parts[3],
                    Product = parts[4],
                    Version = parts[5],
                    Update = parts[6],
                    Edition = parts[7],
                    Language = parts[8],
                    Sw_edition = parts[9],
                    Target_sw = parts[10],
                    Target_hw = parts[11],
                    Other = parts[12]
                };
                _context.CPEItems.Add(cPEItem);
            }
            await _context.SaveChangesAsync();
            return "success";
        }
        // GET: api/CPEItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CPEItem>>> GetCPEItems()
        {
            if (_context.CPEItems == null)
            {
                return NotFound();
            }
            return await _context.CPEItems.ToListAsync();
        }

        [HttpGet("FindBy")]
        public async Task<ActionResult<IEnumerable<CPEItem>>> FindCPEItems(string? Part, string? Vendor, string? Product)
        {
            if (_context.CPEItems == null)
            {
                return NotFound();
            }
            IQueryable<CPEItem> VendorList;
            if (Vendor != null)
            {
                VendorList = Calc.VendorPerfectMatch(Vendor);
                if (VendorList == null)
                {
                    VendorList = Calc.VendorContains(Vendor);
                    if (VendorList == null)
                    { //Correction Attempt here

                        VendorList = _context.CPEItems;
                    }
                }
            }
            else
            {
                Console.WriteLine("NO Vendor Found");
                VendorList = _context.CPEItems;
            }
            if (Part != null) { VendorList = VendorList.Where(s => s.Part == Part); }
            Product = Product.ToLower();
            IQueryable<CPEItem> ProductList;
            string ProductNaked = Regex.Replace(Product, @"[^\p{L}\p{N}]+", " ");
            ProductList = Calc.ProductPerfectMatch(VendorList, Product);
            if (ProductList == null)
            {
                ProductList = Calc.ProductContainsMatch(VendorList, Product);
                if (ProductList == null)
                {
                    ProductList = Calc.ProductPartPerfectMatch(VendorList, Product);
                    if (ProductList == null)
                    {
                        ProductList = Calc.ProductPartPerfectMatch(VendorList, ProductNaked);
                        if (ProductList == null)
                        {
                            ProductList = Calc.ProductPartAllContainsMatch(VendorList, Product);
                            if (ProductList == null)
                            {
                                ProductList = Calc.ProductPartAllContainsMatch(VendorList, ProductNaked);
                                if (ProductList == null)
                                {
                                    ProductList = Calc.ProductPartAnyContainsMatch(VendorList, ProductNaked);

                                    ProductList = Calc.ProductPartAnyContainsMatch(VendorList, Product);
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
            List<CPEItem> ItemList = Calc.OrderInverseAlgorithm(ProductList, Product);
            Calc.evaluateList(Product, ItemList);
            return ItemList;
        }

        [HttpGet("FindLeven/{Product}")]
        public async Task<ActionResult<IEnumerable<CPEItem>>> FindLeven(string? Product)
        {
            if (_context.CPEItems == null)
            {
                return NotFound();
            }
            var res = Calc.recursiveLower(Product, 100);
            res.OrderByDescending(x => FuzzySharp.Fuzz.WeightedRatio(x.Product, Product.Replace(" ", "_").ToLower()));
            if (res != null)
            {
               Calc.evaluateList(Product, res.ToList());
                return res.ToList();
            }
            else
            {
                return NotFound();
            }
        }
        // GET: api/CPEItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CPEItem>> GetCPEItem(int id)
        {
            if (_context.CPEItems == null)
            {
                return NotFound();
            }
            var cPEItem = await _context.CPEItems.FindAsync(id);

            if (cPEItem == null)
            {
                return NotFound();
            }

            return cPEItem;
        }

        /*  // PUT: api/CPEItems/5
          [HttpPut("{id}")]
          public async Task<IActionResult> PutCPEItem(int id, CPEItem cPEItem)
          {
              if (id != cPEItem.Id)
              {
                  return BadRequest();
              }

              _context.Entry(cPEItem).State = EntityState.Modified;

              try
              {
                  await _context.SaveChangesAsync();
              }
              catch (DbUpdateConcurrencyException)
              {
                  if (!Calc.CPEItemExists(id))
                  {
                      return NotFound();
                  }
                  else
                  {
                      throw;
                  }
              }

              return NoContent();
          }

          // POST: api/CPEItems
          [HttpPost]
          public async Task<ActionResult<CPEItem>> PostCPEItem(CPEItem cPEItem)
          {
              if (_context.CPEItems == null)
              {
                  return Problem("Entity set 'CPEContext.CPEItems'  is null.");
              }
              _context.CPEItems.Add(cPEItem);
              await _context.SaveChangesAsync();

              return CreatedAtAction(nameof(GetCPEItem), new { id = cPEItem.Id }, cPEItem);
          }

          // DELETE: api/CPEItems/5
          [HttpDelete("{id}")]
          public async Task<IActionResult> DeleteCPEItem(int id)
          {
              if (_context.CPEItems == null)
              {
                  return NotFound();
              }
              var cPEItem = await _context.CPEItems.FindAsync(id);
              if (cPEItem == null)
              {
                  return NotFound();
              }

              _context.CPEItems.Remove(cPEItem);
              await _context.SaveChangesAsync();

              return NoContent();
          }*/

    }
}
