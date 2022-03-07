using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DevExtremeSuperMarket.Models;

namespace DevExtremeSuperMarket.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SampleOrdersController : Controller
    {
        private SuperMarketDB _context;

        public SampleOrdersController(SuperMarketDB context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var ordres = _context.ordres.Select(i => new {
                i.OrderID,
                i.OrderDate,
                i.CustomerID,
                i.CustomerName,
                i.ShipCountry,
                i.ShipCity
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "OrderID" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(ordres, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SampleOrder();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ordres.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.OrderID });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.ordres.FirstOrDefaultAsync(item => item.OrderID == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.ordres.FirstOrDefaultAsync(item => item.OrderID == key);

            _context.ordres.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(SampleOrder model, IDictionary values) {
            string ORDER_ID = nameof(SampleOrder.OrderID);
            string ORDER_DATE = nameof(SampleOrder.OrderDate);
            string CUSTOMER_ID = nameof(SampleOrder.CustomerID);
            string CUSTOMER_NAME = nameof(SampleOrder.CustomerName);
            string SHIP_COUNTRY = nameof(SampleOrder.ShipCountry);
            string SHIP_CITY = nameof(SampleOrder.ShipCity);

            if(values.Contains(ORDER_ID)) {
                model.OrderID = Convert.ToInt32(values[ORDER_ID]);
            }

            if(values.Contains(ORDER_DATE)) {
                model.OrderDate = Convert.ToDateTime(values[ORDER_DATE]);
            }

            if(values.Contains(CUSTOMER_ID)) {
                model.CustomerID = Convert.ToString(values[CUSTOMER_ID]);
            }

            if(values.Contains(CUSTOMER_NAME)) {
                model.CustomerName = Convert.ToString(values[CUSTOMER_NAME]);
            }

            if(values.Contains(SHIP_COUNTRY)) {
                model.ShipCountry = Convert.ToString(values[SHIP_COUNTRY]);
            }

            if(values.Contains(SHIP_CITY)) {
                model.ShipCity = Convert.ToString(values[SHIP_CITY]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}