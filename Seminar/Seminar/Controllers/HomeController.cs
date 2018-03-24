using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Seminar.Models;

namespace Seminar.Controllers
{
    public class HomeController : Controller
    {
        private DbUpisi_Seminar _db = new DbUpisi_Seminar();


        //Pregled dostupnih seminara
        public ActionResult PregledDostupnihSeminara(string searchBar)
        {
            List<Seminari> sviSeminari = null;
            if (searchBar == null)
            {
                sviSeminari = _db.Seminari.Where(s => s.IsActive && s.Popunjen == false).ToList();
            }
            else
            {
                sviSeminari = _db.Seminari.Where(s => s.IsActive && s.Popunjen == false && s.NazivSeminara.Contains(searchBar)).ToList();
            }
            return View(sviSeminari);
        }

        //Izrada Predbiljezbe
        [HttpGet]
        public ActionResult IzradaPredbiljezbe()
        {
            return View(new Predbiljezbe());
        }

        [HttpPost]
        public ActionResult IzradaPredbiljezbe(Predbiljezbe novaPredbiljezba)
        {
            novaPredbiljezba.DatumPredbiljezbe = DateTime.Now; //pre set()
            novaPredbiljezba.Prihvacen_Odbijen = "neobrađen";
            //add predbiljezbe
            _db.Predbiljezbe.Add(novaPredbiljezba);
            _db.SaveChanges();

            return RedirectToAction("RegistrationCompleted");
        }

        //Potvrda upisa odabranog jezika
        public ActionResult RegistrationCompleted()
        {
            ViewBag.Message = "Hvala što ste upisali odabani seminar !";
            return View();
        }





        //-->USER only(Admin)

        //-------------------------------------------------SEMINARI

        //Pregled svih Seminara  ->User only

        [Authorize(Roles ="User")]
        public ActionResult PregledSeminara()
        {
            return View(_db.SeminariPredbiljezbeCount.ToList());  //SeminariPredbiljezbeCount View-> iz Baze(count-a tek kad je value="P")
        }

        //-------------------------------------------------------------------------Create ->seminar

        [HttpGet]
        [Authorize(Roles ="User")]
        public ActionResult DodajNoviSeminar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DodajNoviSeminar([Bind(Include = "Id,NazivSeminara,DatumPocetka,Opis,IsActive,Popunjen")]Seminari seminari)
        {
            if (ModelState.IsValid)
            {
                _db.Seminari.Add(seminari);
                _db.SaveChanges();
                return RedirectToAction("PregledSeminara");
            }
            return View(seminari);
        }
        //-------------------------------------------------------------------------Edit ->seminar

        [HttpGet]
        [Authorize(Roles ="User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seminari seminari = _db.Seminari.Find(id);
            if (seminari == null)
            {
                return HttpNotFound();
            }
            return View(seminari);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NazivSeminara,DatumPocetka,Opis,IsActive,Popunjen")] Seminari seminari)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(seminari).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("PregledSeminara");
            }
            return View(seminari);
        }

        //-------------------------------------------------------------------------DELETE ->seminar

        //DA bi radilo ...u SSMS-u(right click -->relationship->properties->Insert and Update Specifications ->DELETE(Cascade))

        [Authorize(Roles = "User")]
        public ActionResult Delete(int id)
        {
            Seminari predbiljezbe = _db.Seminari.SingleOrDefault(s =>s.Id==id); 
            _db.Seminari.Remove(predbiljezbe);
            _db.SaveChanges();
            return RedirectToAction("PregledSeminara");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------PREDBILJEZBE
        //------------------------------------------------------------------------------------------------------------------------------------------PREDBILJEZBE
        //Pregled & Obrada Svih Predbiljezbi 

        [Authorize(Roles ="User")]
        public ActionResult UvidAndObradaPredbiljezbi(string searchBar)
        {
            List<Predbiljezbe> svePredbiljezbe = null;
            if (searchBar ==null)
            {
                svePredbiljezbe = _db.Predbiljezbe.Where(p => p.Prihvacen_Odbijen != "B").ToList();
            }
            else
            {
                svePredbiljezbe = _db.Predbiljezbe.Where(p => p.Ime.Contains(searchBar) || p.Prezime.Contains(searchBar) && p.Prihvacen_Odbijen != "B").ToList();
            }
            return View(svePredbiljezbe);
        }

        //Pregled prihvacenih predbiljezbi .(problem je Search=empty ...dobimo listu svePredbijezbe ?--rjesen sa btn ->back to list prihvacenih)
        [Authorize(Roles ="User")]
        public ActionResult prihvacenePredbiljezbe(string searchBar)
        {
            List<Predbiljezbe> sve = null;
            if (searchBar==null)
            {
                sve = _db.Predbiljezbe.Where(p => p.Prihvacen_Odbijen == "P").ToList();
            }
            else
            {
                sve = _db.Predbiljezbe.Where(p => p.Ime.Contains(searchBar) || p.Prezime.Contains(searchBar) && p.Prihvacen_Odbijen == "P").ToList();
            }
            return View(sve);
        }

        //Pregled odbijenih predbiljezbi

        [Authorize(Roles ="User")]
        public ActionResult odbijenePredbiljezbe(string searchBar)
        {
            List<Predbiljezbe> sve = null;
            if (searchBar == null)
            {
                sve = _db.Predbiljezbe.Where(p => p.Prihvacen_Odbijen == "O").ToList();
            }
            else
            {
                sve = _db.Predbiljezbe.Where(p => p.Ime.Contains(searchBar) || p.Prezime.Contains(searchBar) && p.Prihvacen_Odbijen == "O").ToList();
            }
            return View(sve);
        }

        //Pregled Neobrađenih predbiljezbi

        [Authorize(Roles ="User")]
        public ActionResult neobradenePredbijezbe(string searchBar)
        {
            List<Predbiljezbe> sve = null;

            if (searchBar == null)
            {
                sve = _db.Predbiljezbe.Where(p => p.Prihvacen_Odbijen != "P" && p.Prihvacen_Odbijen != "O").ToList();
            }
            else
            {
                sve = _db.Predbiljezbe.Where(p => p.Ime.Contains(searchBar) || p.Prezime.Contains(searchBar) && p.Prihvacen_Odbijen == "neobrađen").ToList();
            }
            return View(sve);
        }


        //Edit Predbiljezbe

        [Authorize(Roles ="User")]
        public ActionResult EditPredbiljezbe(int? id)
        {
            var statusRadio = _db.Predbiljezbe.Where(p => p.Id == id).SingleOrDefault();
            ViewBag.statusPredbiljezbe = statusRadio;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Predbiljezbe predbiljezbe = _db.Predbiljezbe.Find(id);
            if (predbiljezbe == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdSeminar = new SelectList(_db.Seminari, "Id", "NazivSeminara", predbiljezbe.IdSeminar);
            return View(predbiljezbe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPredbiljezbe([Bind(Include = "Id,IdSeminar,DatumPredbiljezbe,Ime,Prezime,Adresa,Mobitel,Email,Prihvacen_Odbijen")] Predbiljezbe predbiljezbe)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(predbiljezbe).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("UvidAndObradaPredbiljezbi");
            }
            ViewBag.IdSeminar = new SelectList(_db.Seminari, "Id", "NazivSeminara", predbiljezbe.IdSeminar);
            return View(predbiljezbe);
        }

        //Delete Predbiljezbe ...
        [Authorize(Roles ="User")]
        public ActionResult DeletePredbiljezbe(int id)
        {
            Predbiljezbe predbiljezbe = _db.Predbiljezbe.Find(id);
            _db.Predbiljezbe.Remove(predbiljezbe);
            _db.SaveChanges();
            return RedirectToAction("UvidAndObradaPredbiljezbi");
        }

        public ActionResult DesignTest()
        {
            return View();
        }
    }
}