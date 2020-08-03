using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MunicipalityTaxApi.Models;

namespace MunicipalityTaxApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipalityTaxController : ControllerBase
    {
        private readonly MunicipalityTaxStoreContext _taxContext;

        public MunicipalityTaxController(MunicipalityTaxStoreContext taxContext)
        {
            _taxContext = taxContext;
        }

        [HttpGet("{name},{date}")]
        public IActionResult Get(string name, DateTime date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(name) || date == null)
            {
                return BadRequest(Constant.EmptyWarning);
            }
            else
            {
                try
                {
                    var taxDetailDaily = _taxContext.MunicipalityTaxDetail
                         .FirstOrDefault(n => n.Name == name && n.Frequency == "daily" && n.StartDate == date && n.EndDate == date);

                    if (taxDetailDaily == null)
                    {
                        var taxDetailWeekly = _taxContext.MunicipalityTaxDetail
                            .FirstOrDefault(n => n.Name == name && n.Frequency == "weekly" && date >= n.StartDate && date <= n.EndDate);

                        if (taxDetailWeekly == null)
                        {
                            var taxDetailMonthly = _taxContext.MunicipalityTaxDetail
                                .FirstOrDefault(n => n.Name == name && n.Frequency == "monthly" && date >= n.StartDate && date <= n.EndDate);
                            if (taxDetailMonthly == null)
                            {
                                var taxDetailYearly = _taxContext.MunicipalityTaxDetail
                            .FirstOrDefault(n => n.Name == name && n.Frequency == "yearly" && date >= n.StartDate && date <= n.EndDate);

                                if (taxDetailYearly != null)
                                    return Ok("Tax for " + name + " : " + taxDetailYearly.Tax.Value);
                            }
                            else
                                return Ok("Tax for " + name + " : " + taxDetailMonthly.Tax.Value);
                        }
                        else
                            return Ok("Tax for " + name + " : " + taxDetailWeekly.Tax.Value);
                    }
                    else
                        return Ok("Tax for " + name + " : " + taxDetailDaily.Tax.Value);
                }
                catch (Exception ex)
                {
                    return BadRequest(Constant.ExceptionMessage + ex.Message);
                }
                return Ok();
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("PostFile")]
        public IActionResult PostFile(IFormFile formFile)
        {
            try
            {
                var uploadedFile = Request.Form.Files[0];
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "files");

                if (uploadedFile.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(uploadedFile.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        uploadedFile.CopyTo(stream);
                        stream.Flush();
                    }
                    var taxes = GetMunicipalityDetails(fullPath);
                    foreach (var tax in taxes)
                    {
                        _taxContext.MunicipalityTaxDetail.Add(tax);
                        _taxContext.SaveChanges();
                    }
                }
                else
                {
                    return BadRequest(Constant.NofileUploaded);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(Constant.ExceptionMessage + ex.Message);
            }
            return Ok(Constant.UploadSuccessMessage);
        }

        private List<MunicipalityTaxDetail> GetMunicipalityDetails(string fullPath)
        {
            List<MunicipalityTaxDetail> taxes = new List<MunicipalityTaxDetail>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        taxes.Add(new MunicipalityTaxDetail()
                        {
                            //Id = Convert.ToInt32(reader.GetValue(0)),
                            Name = reader.GetValue(0).ToString(),
                            MunicipalityId = Convert.ToInt32(reader.GetValue(1)),
                            Frequency = reader.GetValue(2).ToString(),
                            StartDate = Convert.ToDateTime(reader.GetValue(3)),
                            EndDate = Convert.ToDateTime(reader.GetValue(4)),
                            Tax = Convert.ToDouble(reader.GetValue(5))
                        });
                    }
                }
            }
            FileInfo file = new FileInfo(fullPath);
            if (file.Exists)
            {
                file.Delete();
            }
            return taxes;
        }


        [HttpPost]
        [Route("PostTaxData")]
        public IActionResult PostTaxData([FromBody]MunicipalityTaxDetail tax)
        {
            if (tax is null)
            {
                return BadRequest("Tax is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                _taxContext.MunicipalityTaxDetail.Add(tax);
                _taxContext.SaveChanges();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(Constant.ExceptionMessage + ex.Message);
            }
        }

    }
}