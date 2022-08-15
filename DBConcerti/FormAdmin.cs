using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace progettoDB
{
    public partial class FormAdmin : Form
    {
        DataClassesConcertiDataContext db;
        string op1 = "Calcolo dell'incasso di tutti i tour di un artista";
        string op2 = "Conteggio dei biglietti venduti in un tour ";
        string op3 = "Inserimento di un gruppo artista ";
        string op4 = "Inserimento di prezzi relativi a tappa e settore";
        string op5 = "Inserimento di una tappa";

        public FormAdmin(DataClassesConcertiDataContext db)
        {
            InitializeComponent();
            comboBox1.Items.Add(op1);
            comboBox1.Items.Add(op2);
            comboBox1.Items.Add(op3);
            comboBox1.Items.Add(op4);
            comboBox1.Items.Add(op5);
            label1.Hide();
            label2.Hide();
            label3.Hide();
            textBox1.Hide();
            msg.Hide();
            this.db = db;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            msg.Hide();
            if (comboBox1.SelectedItem.Equals(op1))
            {
                var query = from ga in db.GRUPPO_ARTISTA
                            select new { ga.codGruppo, ga.nomeGruppo };
                dataGridView1.MultiSelect = false;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = query.ToList();
                label1.Text = "Gruppi";
                label1.Show();
                label2.Show();
                label2.Text = "Risultati";
                label3.Hide();
                textBox1.Hide();
                dataGridView2.DataSource = null;
            }

            else if (comboBox1.SelectedItem.Equals(op2))
            {

                var query = from t in db.TOUR
                            from ga in db.GRUPPO_ARTISTA
                            where ga.codGruppo == t.codGruppo
                            select new { t.codTour, t.titolo, ga.nomeGruppo };
                dataGridView1.MultiSelect = false;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = query.ToList();
                label1.Text = "Tour";
                label1.Show();
                label2.Show();
                label2.Text = "Risultati";
                textBox1.Hide();
                label3.Hide();
                dataGridView2.DataSource = null;

            }
            else if (comboBox1.SelectedItem.Equals(op5))
            {
                var query = from t in db.TOUR
                            from ga in db.GRUPPO_ARTISTA
                            where ga.codGruppo == t.codGruppo
                            select new { t.codTour, t.titolo, ga.codGruppo, ga.nomeGruppo };
                dataGridView1.MultiSelect = false;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = query.ToList();
                label1.Text = "Tour";
                label1.Show();
                label2.Hide();
                label3.Text = "Data";
                label3.Show();
                textBox1.Show();
                dataGridView2.DataSource = null;


            }

            else if (comboBox1.SelectedItem.Equals(op3))
            {
                textBox1.Show();
                var query = from a in db.ARTISTA
                            orderby a.nomeArte != null descending, a.nomeArte
                            select a;
                dataGridView1.MultiSelect = true;
                dataGridView1.Visible = true;
                dataGridView1.DataSource = query.ToList();
                label1.Text = "Artisti";
                label1.Show();
                label2.Hide();
                label3.Text = "Nome del gruppo";
                label3.Show();
                textBox1.Show();
                dataGridView2.DataSource = null;
            }

            else if (comboBox1.SelectedItem.Equals(op4))
            {
                textBox1.Show();
                var tappe = from tc in db.TAPPA_CONCERTO
                            where tc.data >= DateTime.Today.AddMonths(-1)
                            orderby tc.TOUR.GRUPPO_ARTISTA.nomeGruppo
                            select new { codTour = tc.codTour, data = tc.data, titoloTour = tc.TOUR.titolo, nomeGruppo = tc.TOUR.GRUPPO_ARTISTA.nomeGruppo };
                dataGridView1.DataSource = tappe;

                var settori = from s in db.SETTORE
                              orderby s.LOCATION.CITTA.nomeCitta
                              select new { s.codLocation, s.nomeSettore, s.LOCATION.nomeLocation, s.LOCATION.CITTA.nomeCitta };
                dataGridView2.DataSource = settori;
                label1.Text = "Concerti";
                label1.Show();
                label2.Show();
                label2.Text = "Settori";
                label3.Text = "Prezzo";
                label3.Show();
                textBox1.Show();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.Equals(op1) && dataGridView1.SelectedRows.Count == 1)
            {
                int codice = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var guadagnoSettore = from tour in db.TOUR
                                      from tappa in db.TAPPA_CONCERTO
                                      from ps in db.PREZZO_SETTORE
                                      from s in db.SETTORE
                                      from p in db.POSTO
                                      where tour.codTour == tappa.codTour && ps.data == tappa.data && s.codLocation == p.codLocation && s.nomeSettore == p.nomeSettore && ps.nomeSettore == s.nomeSettore && tour.codGruppo == codice
                                      select new { tour.codTour, tour.titolo, tappa.data, p.nomeSettore, guadagnoSettore = ps.prezzo * (s.POSTO.Count() - ps.numLiberi) };

                var query = from gs in guadagnoSettore.Distinct()
                            group gs by new { gs.codTour, gs.titolo } into x
                            orderby x.Key.codTour
                            select new { codice = x.Key.codTour, titolo = x.Key.titolo, guadagno = x.Sum(l => l.guadagnoSettore) };

                dataGridView2.Visible = true;
                dataGridView2.DataSource = query.ToList();


            }

            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.Equals(op2) && dataGridView1.SelectedRows.Count == 1)
            {
                int codice = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var query = from b in db.BIGLIETTO
                            where b.codTour == codice
                            group b by b.codTour into x
                            select new { numeroBiglietti = x.Count() };
                dataGridView2.Visible = true;
                dataGridView2.DataSource = query.ToList();
            }

            if (comboBox1.SelectedItem != null && textBox1.Text != null && comboBox1.SelectedItem.Equals(op3) && dataGridView1.SelectedRows.Count > 0)
            {
                //inserisco a nuovo indice un gruppo con il nome nella textbox in GRUPPO ARTISTA
                //inserisco a nuovo indice un idgruppo nomeArtista per ogni artista selezionato.
                //ARTISTA a = new ARTISTA();
                var indice = db.GRUPPO_ARTISTA.Count() + 1;
                GRUPPO_ARTISTA ga = new GRUPPO_ARTISTA();
                ga.codGruppo = indice;
                ga.nomeGruppo = textBox1.Text;

                db.GRUPPO_ARTISTA.InsertOnSubmit(ga);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    db.SubmitChanges();
                }

                indice = db.PARTECIPAZIONE.Count() + 1;
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {
                    PARTECIPAZIONE p = new PARTECIPAZIONE();
                    p.codArtista = (int)r.Cells[0].Value;
                    p.codGruppo = ga.codGruppo;
                    db.PARTECIPAZIONE.InsertOnSubmit(p);

                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        db.SubmitChanges();
                    }
                }
                msg.Show();
                msg.Text = "Inserimento effettuato";

            }

            //inserimento prezzo_settore
            if (comboBox1.SelectedItem.Equals(op4) && dataGridView1.SelectedRows.Count == 1 && dataGridView2.SelectedRows.Count == 1)
            {
                var codTour = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var data = dataGridView1.SelectedRows[0].Cells[1].Value;
                var codLocation = (int)dataGridView2.SelectedRows[0].Cells[0].Value;
                var nomeSettore = dataGridView2.SelectedRows[0].Cells[1].Value;
                var prezzi = from ps in db.PREZZO_SETTORE
                             where ps.codTour == codTour && ps.data.Equals(data)
                             select ps;
                prezzi = prezzi.Distinct();
                Console.WriteLine(nomeSettore);
                //se ho zero prezzi inseriti per quella data allora inserisco
                //se ne ho devo controllare che il prezzo-settore che voglio inserire
                //abbia lo stesso codice location di quelli inseriti e un nome settore non inserito
                if (prezzi.Count() == 0 || (prezzi.All(p => p.codLocation == codLocation) && prezzi.All(p => !p.nomeSettore.Equals(nomeSettore))))
                {
                    //inserisco e devo anche contare il numero di posti
                    var numLiberi = (from s in db.SETTORE
                                     where s.codLocation == codLocation && s.nomeSettore.Equals(nomeSettore)
                                     select s).First().POSTO.Count();
                    PREZZO_SETTORE ps = new PREZZO_SETTORE();

                    ps.codTour = codTour;
                    ps.data = Convert.ToDateTime(data);
                    ps.codLocation = codLocation;
                    ps.nomeSettore = nomeSettore.ToString();
                    ps.prezzo = Convert.ToInt32(textBox1.Text);
                    ps.numLiberi = numLiberi;

                    db.PREZZO_SETTORE.InsertOnSubmit(ps);
                    db.SubmitChanges();
                    msg.Show();
                    msg.Text = "Inserimento effettuato";

                }
                else
                {
                    msg.Show();
                    msg.Text = "Inserimento fallito";
                }

            }
            if (comboBox1.SelectedItem.Equals(op5) && !string.IsNullOrEmpty(textBox1.Text) && dataGridView1.SelectedRows.Count ==1 && DateTime.TryParse(textBox1.Text, out DateTime boh))
            {
                msg.Hide();
                var codGruppo = (int) dataGridView1.SelectedRows[0].Cells[2].Value;
                var codTour = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var data = DateTime.Parse(textBox1.Text);
                var ok = data > DateTime.Today;
                Console.WriteLine(DateTime.Today);
                if (ok)
                {
                    
                    //guardo quali artisti fanno parte del gruppo
                    var artisti = from p in db.PARTECIPAZIONE
                                   where p.codGruppo == codGruppo
                                   select new { p.codArtista };

                    //guardo di quali gruppi fanno parte gli artisti
                    var gruppi = from p in db.PARTECIPAZIONE
                                 from a in artisti
                                 where a.codArtista == p.codArtista
                                 select new { p.codGruppo};

                    //guardo se il tour su cui sto lavorando ha delle tappe
                    var tappeTour = from tc in db.TAPPA_CONCERTO
                                    where tc.codTour == codTour
                                    select tc;
                    if (!tappeTour.Any())
                    {
                        //Se non ho tappe controllo se gli artisti del gruppo sono impegnati in esibizioni in quel giorno
                        var esibizioni = from es in db.ESIBIZIONE
                                         from g in gruppi.Distinct()
                                         where es.codGruppo == g.codGruppo && es.codTour != codTour && es.data.Equals(data)
                                         select new { es.codTour, es.data };
                        if (!esibizioni.Any())
                        {
                            //se non ho esibizioni quel giorno posso inserire
                            TAPPA_CONCERTO tc = new TAPPA_CONCERTO();
                            tc.codTour = codTour;
                            tc.data = data;
                            tc.ora = TimeSpan.Parse("21:00");

                            db.TAPPA_CONCERTO.InsertOnSubmit(tc);
                            db.SubmitChanges();
                            msg.Text = "Inserimento riuscito";
                            msg.Show();

                        }
                        else
                        {
                            msg.Text = "Inserimento fallito";
                            msg.Show();
                        }

                                         
                    }
                    else
                    {
                        //Se ho tappe devo guardare se è il tour (che coinvolge quegli artisti) con la data più alta
                        //cioè il tour di cui posso aggiungere date
                        var tourDaAggiornare = from tc in db.TAPPA_CONCERTO
                                               from t in db.TOUR
                                               from g in gruppi.Distinct()
                                               where tc.codTour == t.codTour && g.codGruppo == t.codGruppo
                                               orderby tc.data descending
                                               select new { t.codTour, tc.data };

                        if (tourDaAggiornare.First().codTour == codTour && tourDaAggiornare.First().data < data)
                        {
                            //se sono il tour da aggiornare
                            //guardo se ci sono esibizioni in quella data
                            var esibizioni = from es in db.ESIBIZIONE
                                             from g in gruppi.Distinct()
                                             where es.codGruppo == g.codGruppo && es.codTour != codTour && es.data.Equals(data)
                                             select es;
                            if (!esibizioni.Any())
                            {
                                msg.Text = "Inserimento riuscito";
                                TAPPA_CONCERTO tc = new TAPPA_CONCERTO();
                                tc.codTour = codTour;
                                tc.data = data;
                                tc.ora = TimeSpan.Parse("21:00");

                                db.TAPPA_CONCERTO.InsertOnSubmit(tc);
                                db.SubmitChanges();
                                msg.Show();
                            }
                            else
                            {
                                msg.Text = "Inserimento fallito";
                                msg.Show();
                            }
                        }
                        else
                        {   
                            msg.Text = "Inserimento fallito";
                            msg.Show();
                        }



                    }
                    /*var componentiGruppo = from p in db.PARTECIPAZIONE
                                           where p.codGruppo == codGruppo
                                           select p;
                    var gruppi = from cg in componentiGruppo
                                 from ga in db.GRUPPO_ARTISTA
                                 where ga.codGruppo == cg.codGruppo
                                 select new { ga.codGruppo };
                    gruppi = gruppi.Distinct();

                    //tutte le tappe che coinvolgono gli stessi artisti nel tour che voglio inserire
                    var tutteTappe =from tc in db.TAPPA_CONCERTO
                                    from g in gruppi
                                    where tc.TOUR.GRUPPO_ARTISTA.codGruppo == g.codGruppo
                                    select tc;
                    //solo le tappe del tour che voglio inserire.
                    var tappe = from t in tutteTappe
                                where t.codTour == codTour
                                select t;
                    
                    if (tappe == null)
                    {
                        //se non ho nessuna tappa per quel tour guardo che sia maggiore delle altre date dell'artista
                        var dateMaggiori = from t in tutteTappe
                                           where t.data >= data
                                           select t;

                        if (dateMaggiori == null)
                        {   //se non ho date maggiori e non ho alcuna esibizione in quella data
                            var esibizioni = from es in db.ESIBIZIONE
                                             from g in gruppi
                                             where es.codTour != codTour && es.data.Equals(data) && es.GRUPPO_ARTISTA.codGruppo == g.codGruppo
                                             select es;
                            if (esibizioni == null)
                            {
                                TAPPA_CONCERTO tc = new TAPPA_CONCERTO();
                                tc.codTour = codTour;
                                tc.data = data;
                                tc.ora = TimeSpan.Parse("21:00");

                                db.TAPPA_CONCERTO.InsertOnSubmit(tc);
                                db.SubmitChanges();
                                msg.Text = "Inserimento effettuato";
                            }
                            
                        }

                    }
                    else
                    {   //se ho delle tappe devo controllare di essere il tour con la data maggiore dell'artista
                        var dateMaggiori = from t in tutteTappe
                                           orderby t.data descending
                                           select t;
                        var inserisci = dateMaggiori.First().codTour == codTour;
                        if (inserisci && data > dateMaggiori.First().data)
                        {
                            //devo controllare che i gruppi non siano coinvolti in esibizioni quel giorno 
                            var esibizioni = from es in db.ESIBIZIONE
                                             from g in gruppi
                                             where es.codTour != codTour && es.data.Equals(data) && es.GRUPPO_ARTISTA.codGruppo == g.codGruppo
                                             select es;
                            TAPPA_CONCERTO tc = new TAPPA_CONCERTO();
                            tc.codTour = codTour;
                            tc.data = data;
                            tc.ora = TimeSpan.Parse("21:00");

                            db.TAPPA_CONCERTO.InsertOnSubmit(tc);
                            db.SubmitChanges();
                            msg.Text = "Inserimento effettuato";
                        }
                        else
                        {
                            msg.Text = "Inserimento fallito";
                        }
                    }*/
                }
            }
        }
    }
}
