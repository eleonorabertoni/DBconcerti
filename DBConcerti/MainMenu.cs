using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace progettoDB
{
    public partial class MainMenu : Form
    {
        DataClassesConcertiDataContext db;
        Form adminForm;
        FormUser userForm;
        public MainMenu()
        {
            InitializeComponent();
            db = new DataClassesConcertiDataContext();
            adminForm = new FormAdmin(db);
            userForm = new FormUser(db);

        }

        private void buttonAdmin_Click(object sender, EventArgs e)
        {  
             adminForm.Show();
        }

        private void buttonUser_Click(object sender, EventArgs e)
        {    
            if (!String.IsNullOrEmpty(mailInput.Text) && !String.IsNullOrEmpty(passwordInput.Text))
            {
                var utenti = from u in db.UTENTE
                             where u.mail.Equals(mailInput.Text.ToString())
                             select u;
                var p = utenti.First().password.Trim();
                if (p.Equals(passwordInput.Text.ToString()))
                {
                    userForm.Show();
                    userForm.CodUtente = utenti.First().codUtente;
                }
            }

            Console.WriteLine(userForm.CodUtente);
             
            
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
