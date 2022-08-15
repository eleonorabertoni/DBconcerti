using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace progettoDB
{
    public partial class FormUser : Form
    {
        DataClassesConcertiDataContext db;
        public int CodUtente {
            get;
            set;
        }
        string op1 = "Visualizzazione dei prossimi concerti in una determinata città";
        string op2 = "Visualizzazione dei prossimi concerti di un determinato artista";
        string op3 = "Visualizzazione dei posti liberi per settore per un determinato concerto";
        string op4 = "Acquisto di biglietti per concerto e settore scelti";
        string op5 = "Visualizzazione di tutte le esibizioni di un artista in cui è presente un video";
        string op6 = "Visualizzazione della scaletta di un concerto";
        string op7 = "Visualizzazione biglietti acquistati";
        public FormUser(DataClassesConcertiDataContext db)
        {
            InitializeComponent();
            queryCB.Items.Add(op1);
            queryCB.Items.Add(op2);
            queryCB.Items.Add(op3);
            queryCB.Items.Add(op4);
            queryCB.Items.Add(op5);
            queryCB.Items.Add(op6);
            queryCB.Items.Add(op7);
            this.db = db;
            menu1.Hide();
            menu2.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            msg.Hide();
            userInput.Hide();
            dataGridView1.MultiSelect = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            msg.Hide();
            //input op1 -> menù tendina con le città
            if (queryCB.SelectedItem.Equals(op1))
            {
                menu1.SelectedItem = null;
                menu1.Items.Clear();
                var cit = from c in db.CITTA
                          orderby c.nomeCitta
                          select new { c.nomeCitta };
                foreach (var c in cit.Distinct())
                {
                    menu1.Items.Add(c.nomeCitta);
                }

                menu1.Show();
                label1.Text = "Città";
                label1.Show();
                label2.Hide();
                label3.Hide();
                menu2.Hide();
                userInput.Hide();
            }

            //input op3 e op6 -> menu tendina con gruppi e poi con date-città
            if (queryCB.SelectedItem.Equals(op3) || queryCB.SelectedItem.Equals(op6))
            {
                menu1.SelectedItem = null;
                menu1.Items.Clear();
                menu2.SelectedItem = null;
                menu2.Items.Clear();
                var gruppi = from ga in db.GRUPPO_ARTISTA
                             orderby ga.nomeGruppo
                             select new { ga.nomeGruppo };
                foreach (var ga in gruppi.Distinct())
                {
                    menu1.Items.Add(ga.nomeGruppo);
                }

                menu1.Show();
                menu2.Show();
                label1.Text = "Artista";
                label2.Text = "Date";
                label1.Show();
                label2.Show();
                label3.Hide();
                userInput.Hide();
            }

            //input op5, op2 -> 1 menu tendina gruppi
            if (queryCB.SelectedItem.Equals(op2) || queryCB.SelectedItem.Equals(op5))
            {
                menu1.SelectedItem = null;
                menu1.Items.Clear();
                var gruppi = from ga in db.GRUPPO_ARTISTA
                             orderby ga.nomeGruppo
                             select new { ga.nomeGruppo };
                foreach (var ga in gruppi.Distinct())
                {
                    menu1.Items.Add(ga.nomeGruppo);
                }
                menu1.Show();
                menu2.Hide();
                label1.Text = "Artista";
                label1.Show();
                label2.Hide();
                label3.Hide();
                userInput.Hide();

            }

            //menu tendina con gruppi + tutti i menu
            if (queryCB.SelectedItem.Equals(op4))
            {
                menu1.SelectedItem = null;
                menu2.SelectedItem = null;
                menu1.Items.Clear();
                menu2.Items.Clear();

                var gruppi = from ga in db.GRUPPO_ARTISTA
                             orderby ga.nomeGruppo
                             select new { ga.nomeGruppo };
                foreach (var ga in gruppi.Distinct())
                {
                    menu1.Items.Add(ga.nomeGruppo);
                }

                menu1.Show();
                menu2.Show();
                label1.Text = "Artista";
                label2.Text = "Date";
                label3.Text = "Quantità";
                label1.Show();
                label2.Show();
                label3.Show();
                userInput.Show();

            }

            //nascondo tutto
            if (queryCB.SelectedItem.Equals(op7))
            {
                menu1.Hide();
                menu2.Hide();
                userInput.Hide();
                label1.Hide();
                label2.Hide();
                label3.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {   
            if (queryCB.SelectedItem != null)
            {

                if (queryCB.SelectedItem.Equals(op1) && menu1.SelectedItem != null)
                {
                    var nomeCitt = menu1.SelectedItem.ToString();
                    var query = from c in db.CITTA
                                from tc in db.TAPPA_CONCERTO
                                from ga in db.GRUPPO_ARTISTA
                                from t in db.TOUR
                                from l in db.LOCATION
                                from s in db.SETTORE
                                from ps in db.PREZZO_SETTORE
                                where c.nomeCitta == nomeCitt && c.codCitta == l.codCitta && s.codLocation == l.codLocation && ps.codLocation == s.codLocation && ps.nomeSettore == s.nomeSettore && ps.codTour == tc.codTour && ps.data == tc.data && tc.codTour == t.codTour && t.codGruppo == ga.codGruppo && ps.data >= DateTime.Today
                                select new { t.titolo, ga.nomeGruppo, ps.data };
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = query.Distinct().OrderBy(el => el.data).ToList();

                }

                if (queryCB.SelectedItem.Equals(op2) && menu1.SelectedItem != null)
                {
                    var nomeGruppo = menu1.SelectedItem;
                    var query = from c in db.CITTA
                                from tc in db.TAPPA_CONCERTO
                                from ga in db.GRUPPO_ARTISTA
                                from t in db.TOUR
                                from l in db.LOCATION
                                from s in db.SETTORE
                                from ps in db.PREZZO_SETTORE
                                where ga.nomeGruppo.Equals(nomeGruppo) && c.codCitta == l.codCitta && s.codLocation == l.codLocation && ps.codLocation == s.codLocation && ps.nomeSettore == s.nomeSettore && ps.codTour == tc.codTour && ps.data == tc.data && tc.codTour == t.codTour && t.codGruppo == ga.codGruppo && ps.data >= DateTime.Today
                                select new { t.titolo, ps.data, c.nomeCitta };
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = query.Distinct().ToList();

                
                }

                if (queryCB.SelectedItem.Equals(op3) && menu1.SelectedItem != null & menu2.SelectedItem != null)
                {
                       var tour =   from t in db.TOUR
                                    from ga in db.GRUPPO_ARTISTA
                                    where ga.nomeGruppo.Equals(menu1.SelectedItem) && t.codGruppo == ga.codGruppo
                                    select new { t.codTour };
                        var codici = tour.ToList();

                        var temp = menu2.SelectedItem.ToString().IndexOf(' ');
                        var data = menu2.SelectedItem.ToString().Remove(temp, menu2.SelectedItem.ToString().Length-temp);

                        var query = from tc in db.TAPPA_CONCERTO
                                    from ps in db.PREZZO_SETTORE
                                    from t in tour
                                    where t.codTour == tc.codTour  && tc.data == Convert.ToDateTime(data) && tc.codTour == ps.codTour && tc.data == ps.data 
                                    select new { ps.nomeSettore, ps.prezzo, ps.numLiberi };
                        dataGridView1.Visible = true;
                        dataGridView1.DataSource = query.Distinct().ToList();


                }

                if(queryCB.SelectedItem.Equals(op5) && menu1.SelectedItem != null)
                {
                    var nomeg = menu1.SelectedItem;
                    var query = from ga in db.GRUPPO_ARTISTA
                                from es in db.ESIBIZIONE
                                from tc in db.TAPPA_CONCERTO
                                from b in db.BRANO
                                where es.link_video != null && ga.nomeGruppo.Equals(nomeg) && ga.codGruppo == es.codGruppo && es.codTour == tc.codTour && tc.data == es.data && es.codBrano == b.codBrano
                                orderby tc.data ascending
                                select new { ga.nomeGruppo, b.titolo, tc.data };
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = query.ToList();

                }

                if (queryCB.SelectedItem.Equals(op6) && menu1.SelectedItem != null && menu2.SelectedItem != null)
                {
                    var tour = from t in db.TOUR
                               from ga in db.GRUPPO_ARTISTA
                               where ga.nomeGruppo.Equals(menu1.SelectedItem) && t.codGruppo == ga.codGruppo
                               select new { t.codTour };
                    var codici = tour.ToList();

                    var temp = menu2.SelectedItem.ToString().IndexOf(' ');
                    var data = menu2.SelectedItem.ToString().Remove(temp, menu2.SelectedItem.ToString().Length - temp);

                    var query = from ga in db.GRUPPO_ARTISTA
                                from es in db.ESIBIZIONE
                                from tc in db.TAPPA_CONCERTO
                                from b in db.BRANO
                                from t in tour
                                where tc.data == Convert.ToDateTime(data) && tc.codTour == t.codTour && ga.codGruppo == es.codGruppo && es.codTour == tc.codTour && tc.data == es.data && es.codBrano == b.codBrano
                                orderby es.ordine
                                select new { es.ordine, ga.nomeGruppo, b.titolo };

                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = query.ToList();

                }

                if(queryCB.SelectedItem != null && queryCB.SelectedItem.Equals(op4) && menu1.SelectedItem != null && menu2.SelectedItem != null && dataGridView1.SelectedRows.Count == 1 && !String.IsNullOrEmpty(userInput.Text))
                {   //mi serve username per arrivare al codice utente ma dove inserisco utente???
                    //per ora li metto fissi
                    //acquisto il biglietto
                    msg.Show();
                    if (userInput.Text.All(c => char.IsDigit(c)))
                    {   var maxBiglietti = (int)dataGridView1.SelectedRows[0].Cells[2].Value;
                        int nBiglietti = Convert.ToInt32(userInput.Text);
                       
                        if (nBiglietti > 0 && nBiglietti <= maxBiglietti)
                        {
                            
                            //acquisto del biglietto
                            //nel menu1 ho nomegruppo
                            //nel menu2 ho data
                            //selezionato ho il nome settore e il prezzo
                            //nel biglietto devo mettere il codice del posto, il primo libero nel settore
                            //codiceUtente
                            //codice tour e data che trovo nelle tappe e tour
                            var temp = menu2.SelectedItem.ToString().IndexOf(' ');
                            var data = menu2.SelectedItem.ToString().Remove(temp, menu2.SelectedItem.ToString().Length - temp);
                            var nCit = menu2.SelectedItem.ToString().Remove(0, temp+1);
                            Console.WriteLine(nCit);
                            var nSet = dataGridView1.SelectedRows[0].Cells[0].Value;
                            
                            //prendo il tour
                            var tour = from ga in db.GRUPPO_ARTISTA
                                       from t in db.TOUR
                                       from tc in db.TAPPA_CONCERTO
                                       where ga.nomeGruppo.Equals(menu1.SelectedItem) && ga.codGruppo == t.codGruppo
                                       select new { t.codTour };
                            Console.WriteLine(tour.First().codTour);
                            //prendo la tappa
                            var tappa = from tc in db.TAPPA_CONCERTO
                                        from t in tour
                                        where t.codTour == tc.codTour && tc.data.Equals(Convert.ToDateTime(data))
                                        select new { t.codTour, tc.data };
                            Console.WriteLine(tappa.First().codTour);
                            Console.WriteLine(tappa.First().data);

                            //prendo l'identificatore del settore scelto
                            var settore = from c in db.CITTA
                                          from l in db.LOCATION
                                          from s in db.SETTORE
                                          where c.codCitta == l.codCitta && s.codLocation == l.codLocation && c.nomeCitta.Equals(nCit) && s.nomeSettore.Equals(nSet)
                                          select new { s.codLocation, s.nomeSettore };
                            
                            var prezzi = from t in tappa
                                         from s in settore
                                         from ps in db.PREZZO_SETTORE
                                         where ps.codTour == t.codTour && ps.data.Equals(t.data) && ps.codLocation == s.codLocation && ps.nomeSettore.Equals(s.nomeSettore)
                                         select ps;
                            //creazione dei biglietti
                            var prezzo = prezzi.Distinct().First();
                            var codBiglietto = db.BIGLIETTO.Count() + 1;

                            var posto = from p in prezzi
                                        from s in db.SETTORE
                                        from po in db.POSTO
                                        where p.codLocation == s.codLocation && p.nomeSettore.Equals(s.nomeSettore) && p.codLocation == po.codLocation && p.nomeSettore == po.nomeSettore 
                                        select new { po.codPosto, po.numPosto};
                            //nPosto 
                            int nPosto = posto.Distinct().Count() - prezzo.numLiberi + 1;
                            Console.WriteLine("POSTI: "+nPosto);
                            for (int i= 0; i < nBiglietti; i++)
                            {
                                BIGLIETTO b = new BIGLIETTO();
                                b.codBiglietto = codBiglietto + i;
                                b.codUtente = CodUtente;
                                b.codTour = prezzo.codTour;
                                b.data = prezzo.data;

                                var postoPrenota =   from po in posto
                                                     where po.numPosto == nPosto + i
                                                     select new { po.codPosto };
                                b.codPosto = postoPrenota.First().codPosto;

                                db.BIGLIETTO.InsertOnSubmit(b);



                            }
                            //aggiornamento del numero di posti liberi
                            prezzo.numLiberi = prezzo.numLiberi - nBiglietti;
                            db.SubmitChanges();
                            msg.Text = "ACQUISTO EFFETTUATO, TOTALE: " + nBiglietti*prezzo.prezzo;

                        }
                        else
                        {
                            msg.Text = "impossibile acquisto";
                        }
                    }
                    else
                    {
                        msg.Text = "INSERIRE NUMERO DI BIGLIETTI";
                    }
                    

                }

                //visualizza biglietti
                if (queryCB.SelectedItem.Equals(op7))
                {
                    var query = from b in db.BIGLIETTO
                                from u in db.UTENTE
                                where b.codUtente == u.codUtente && u.codUtente == CodUtente
                                select new { tour=b.TAPPA_CONCERTO.TOUR.titolo, artista=b.TAPPA_CONCERTO.TOUR.GRUPPO_ARTISTA.nomeGruppo, data=b.data, città=b.POSTO.SETTORE.LOCATION.CITTA.nomeCitta, location=b.POSTO.SETTORE.LOCATION.nomeLocation,settore=b.POSTO.nomeSettore, posto=b.POSTO.numPosto, };
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = query;

                }
            }

        }

        private void menu1_SelectedIndexChanged(object sender, EventArgs e)
        {
            menu2.SelectedItem = null;
            menu2.Items.Clear();
            if (queryCB.SelectedItem.Equals(op3) || queryCB.SelectedItem.Equals(op4))
            {
                //nel menu2 faccio comparire le date future e con la città
                var nomeGruppo = menu1.SelectedItem;
                var date =  from c in db.CITTA
                            from tc in db.TAPPA_CONCERTO
                            from ga in db.GRUPPO_ARTISTA
                            from t in db.TOUR
                            from l in db.LOCATION
                            from s in db.SETTORE
                            from ps in db.PREZZO_SETTORE
                            where ga.nomeGruppo.Equals(nomeGruppo) && c.codCitta == l.codCitta && s.codLocation == l.codLocation && ps.codLocation == s.codLocation && ps.nomeSettore == s.nomeSettore && ps.codTour == tc.codTour && ps.data == tc.data && tc.codTour == t.codTour && t.codGruppo == ga.codGruppo && ps.data >= DateTime.Today
                            select new {ps.data, c.nomeCitta };
                foreach (var d in date.Distinct())
                {
                    menu2.Items.Add(d.data.ToString("yyyy-MM-dd") +' '+d.nomeCitta);
                }
               

            }

            if (queryCB.SelectedItem.Equals(op6))
            {
                var nomeGruppo = menu1.SelectedItem;
                var date = from c in db.CITTA
                           from tc in db.TAPPA_CONCERTO
                           from ga in db.GRUPPO_ARTISTA
                           from t in db.TOUR
                           from l in db.LOCATION
                           from s in db.SETTORE
                           from ps in db.PREZZO_SETTORE
                           where ga.nomeGruppo.Equals(nomeGruppo) && c.codCitta == l.codCitta && s.codLocation == l.codLocation && ps.codLocation == s.codLocation && ps.nomeSettore == s.nomeSettore && ps.codTour == tc.codTour && ps.data == tc.data && tc.codTour == t.codTour && t.codGruppo == ga.codGruppo 
                           select new { ps.data, c.nomeCitta };
                foreach (var d in date.Distinct())
                {
                    menu2.Items.Add(d.data.ToString("yyyy-MM-dd") + ' ' + d.nomeCitta);
                }
            }


        }

        private void menu2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(menu2.SelectedItem != null && queryCB.SelectedItem != null && queryCB.SelectedItem.Equals(op4) && menu1.SelectedItem != null)
            {
                    var tour =   from t in db.TOUR
                                    from ga in db.GRUPPO_ARTISTA
                                    where ga.nomeGruppo.Equals(menu1.SelectedItem) && t.codGruppo == ga.codGruppo
                                    select new { t.codTour };
                        var codici = tour.ToList();

                        var temp = menu2.SelectedItem.ToString().IndexOf(' ');
                        var data = menu2.SelectedItem.ToString().Remove(temp, menu2.SelectedItem.ToString().Length-temp);

                        var query = from tc in db.TAPPA_CONCERTO
                                    from ps in db.PREZZO_SETTORE
                                    from t in tour
                                    where t.codTour == tc.codTour  && tc.data == Convert.ToDateTime(data) && tc.codTour == ps.codTour && tc.data == ps.data 
                                    select new { ps.nomeSettore, ps.prezzo, ps.numLiberi };
                        dataGridView1.Visible = true;
                        dataGridView1.DataSource = query.Distinct().ToList();
            }
        }
    }
}
