using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using WepAPPCRUD.Models;

namespace WepAPPCRUD.Controllers
{
    public class CRUDController : Controller
    {
        // GET: CRUD
        public ActionResult Index()
        {
            List<CRUDModel> lstProject = new List<CRUDModel>();
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/XML/CRUDXMLFile.xml"));
            DataView dvPrograms;
            dvPrograms = ds.Tables[0].DefaultView;
            dvPrograms.Sort = "Id";

            foreach (DataRowView dr in dvPrograms)
            {
                CRUDModel model = new CRUDModel();
                model.Id = Convert.ToInt32(dr[0]);

                model.Name = Convert.ToString(dr[1]);

                model.Surname = Convert.ToString(dr[2]);
                model.CellphoneNo = Convert.ToString(dr[3]);
                lstProject.Add(model);
            }
            if (lstProject.Count > 0)
            {
                return View(lstProject);
            }
            return View();



            //return View();
        }
        CRUDModel model = new CRUDModel();
        public ActionResult AddEditUser(int? id)
        {

            int Id = Convert.ToInt32(id);
            if (Id > 0)
            {
                GetDetailsById(Id);
                model.IsEdit = true;
                return View(model);
            }
            else
            {
                model.IsEdit = false;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult AddEditUser(CRUDModel mdl)
        {


            if (mdl.Id > 0)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                var items = (from item in xmlDoc.Descendants("User")
                             select item).ToList();
                //Check if ID exist
                XElement selected = items.Where(p => p.Element("Id").Value == mdl.Id.ToString()).FirstOrDefault();

                //Check if combination of username and password exist
                XElement xUserExist = items.Where(p => p.Element("Name").Value == mdl.Name.ToString() && p.Element("Surname").Value == mdl.Surname.ToString()).FirstOrDefault();

                selected.Remove();
                xmlDoc.Save(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                xmlDoc.Element("Users").Add(new XElement("User",
                                                                new XElement("Id", mdl.Id),
                                                                new XElement("Name", mdl.Name),

                                                                new XElement("Surname", mdl.Surname),

                                                                new XElement("CellphoneNo", mdl.CellphoneNo)

                                                           )
                                            );
                xmlDoc.Save(Server.MapPath("~/XML/CRUDXMLFile.xml"));

                return RedirectToAction("Index", "CRUD");
            }
            else
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));

                //Check if name and surmane already exist before insert
                XDocument xmlDocUpdate = XDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                var items = (from item in xmlDocUpdate.Descendants("User")
                             select item).ToList();

                XElement xUserExist = items.Where(p => p.Element("Name").Value == mdl.Name.ToString() && p.Element("Surname").Value == mdl.Surname.ToString()).FirstOrDefault();
                if (xUserExist==null)
                {
                    XmlNodeList nodelist = oXmlDocument.GetElementsByTagName("User");
                    var x = oXmlDocument.GetElementsByTagName("Id");
                    int Max = 0;
                    foreach (XmlElement item in x)
                    {
                        int EId = Convert.ToInt32(item.InnerText.ToString());
                        if (EId > Max)
                        {
                            Max = EId;
                        }
                    }
                    Max = Max + 1;
                    XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                    xmlDoc.Element("Users").Add(new XElement("User",
                                                                    new XElement("Id", Max),

                                                                   new XElement("Name", mdl.Name),

                                                                   new XElement("Surname", mdl.Surname),

                                                                   new XElement("CellphoneNo", mdl.CellphoneNo)
                                                               )
                                                               );
                    xmlDoc.Save(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                }
                else
                {
                    ViewBag.SuccessMessage = "<p>User already exist!</p>";
                }

                return RedirectToAction("Index", "CRUD");
            }

        }




        public ActionResult Delete(int Id)
        {
            if (Id > 0)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));
                var items = (from item in xmlDoc.Descendants("User")
                             select item).ToList();
                XElement selected = items.Where(p => p.Element("Id").Value == Id.ToString()).FirstOrDefault();
                selected.Remove();
                xmlDoc.Save(Server.MapPath("~/XML/CRUDXMLFile.xml"));
            }

            return RedirectToAction("Index", "CRUD");

        }



        public void GetDetailsById(int Id)
        {
            XDocument oXmlDocument = XDocument.Load(Server.MapPath("~/XML/CRUDXMLFile.xml"));
            var items = (from item in oXmlDocument.Descendants("User")
                         where Convert.ToInt32(item.Element("Id").Value) == Id
                         select new projectItems
                         {
                             Id = Convert.ToInt32(item.Element("Id").Value),

                             Name = item.Element("Name").Value,

                             Surname = item.Element("Surname").Value,

                             CellphoneNo = item.Element("CellphoneNo").Value,

                         }).SingleOrDefault();

            if (items != null)
            {
                model.Id = items.Id;

                model.Name = items.Name;

                model.Surname = items.Surname;

                model.CellphoneNo = items.CellphoneNo;
            }
        }

        public class projectItems
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Surname { get; set; }

            public string CellphoneNo { get; set; }
            public projectItems()
            {
            }
        }

    }
}