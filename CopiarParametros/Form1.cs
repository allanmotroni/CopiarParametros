using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CopiarParametros
{
    public partial class Form1 : Form
    {

        private const string ARQUIVO_XML = "CopiarParametros.xml";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                txtDescricao.KeyPress += TextBox_KeyPress;
                txtValor.KeyPress += TextBox_KeyPress;
                txtValor.Visible = false;

                AbrirXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AbrirXML()
        {
            try
            {
                using (DataSet dsXml = new DataSet())
                {
                    dsXml.ReadXml(ARQUIVO_XML);
                    if (dsXml.Tables.Count > 0)
                    {
                        CarregarObjetosXML(dsXml.Tables[0]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CarregarObjetosXML(DataTable dt)
        {
            List<Objeto> lista = dt.AsEnumerable().Select(p => new Objeto()
            {
                Descricao = p.Field<string>("Descricao"),
                Valor = p.Field<string>("Valor")
            }).ToList();

            lstObjetos.Items.Clear();

            foreach (Objeto objeto in lista)
            {
                IncluirNaLista(objeto);
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(e.KeyChar) == 13)
                {
                    if (((TextBox)sender).Name.ToUpper() == "TXTDESCRICAO")
                    {
                        txtValor.Focus();
                    }
                    else if (((TextBox)sender).Name.ToUpper() == "TXTVALOR")
                    {
                        btnIncluir.PerformClick();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            try
            {
                Incluir();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Incluir()
        {
            try
            {
                var objeto = Validar();
                Cadastrar(objeto);
                Limpar();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void lstObjetos_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (lstObjetos.SelectedIndex > -1)
                {
                    var objeto = (Objeto)lstObjetos.SelectedItem;
                    Clipboard.SetText(objeto.Valor);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Cadastrar(Objeto objeto)
        {
            try
            {
                IncluirNaLista(objeto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GravarArquivo()
        {
            try
            {
                List<Objeto> objetos = ObterObjetos();
                string caminho = ARQUIVO_XML;

                using (XmlTextWriter w = new XmlTextWriter(caminho, Encoding.Default))
                {
                    w.WriteStartDocument(true);
                    w.Formatting = Formatting.Indented;
                    w.Indentation = 2;
                    w.WriteStartElement("Objetos");

                    foreach (Objeto objeto in objetos)
                    {
                        w.WriteStartElement("Objeto");

                        w.WriteStartElement("Descricao");
                        w.WriteString(objeto.Descricao);
                        w.WriteEndElement();

                        w.WriteStartElement("Valor");
                        w.WriteString(objeto.Valor);
                        w.WriteEndElement();

                        w.WriteEndElement();
                    }
                    w.WriteEndDocument();
                    w.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void IncluirNaLista(Objeto objeto)
        {
            try
            {
                lstObjetos.ValueMember = "Valor";
                lstObjetos.DisplayMember = "Descricao";
                lstObjetos.Items.Add(objeto);
                AtualizarTotal();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AtualizarTotal()
        {
            try
            {
                toolStripStatusTotal.Text = lstObjetos.Items.Count.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Objeto Validar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDescricao.Text))
                    throw new Exception("Favor preencher o campo Descrição!");

                if (string.IsNullOrWhiteSpace(txtValor.Text))
                    throw new Exception("Favor preencher o campo Valor!");

                return new Objeto()
                {
                    Descricao = txtDescricao.Text.Trim(),
                    Valor = txtValor.Text.Trim()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Limpar()
        {
            try
            {
                txtDescricao.Clear();
                txtValor.Clear();
                txtDescricao.Focus();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkTopMost.Checked;
        }

        private void lstObjetos_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && lstObjetos.SelectedIndex > -1)
                {
                    var objeto = (Objeto)lstObjetos.SelectedItem;
                    ToolStripMenuItemValor.Text = objeto.Valor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSalvarXML_Click(object sender, EventArgs e)
        {
            try
            {                
                GravarArquivo();
                MessageBox.Show("Arquivo salvo com sucesso!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private List<Objeto> ObterObjetos()
        {
            try
            {
                List<Objeto> objetos = new List<Objeto>();
                foreach (var item in lstObjetos.Items)
                {
                    objetos.Add((Objeto)item);
                }
                return objetos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void lstObjetos_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete && lstObjetos.SelectedIndex > -1)
                {
                    RemoverDaLista((Objeto)lstObjetos.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoverDaLista(Objeto objeto)
        {
            try
            {
                lstObjetos.Items.Remove(objeto);
                AtualizarTotal();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void lstObjetos_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (lstObjetos.SelectedIndex > -1)
                {
                    Objeto objeto = (Objeto)lstObjetos.SelectedItem;
                    PreencherCampos(objeto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PreencherCampos(Objeto objeto)
        {
            if (objeto != null)
            {
                txtValor.Text = objeto.Valor;
                txtDescricao.Text = objeto.Descricao;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            try
            {
                Limpar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstObjetos.SelectedIndex > -1)
                {
                    Objeto objetoOld = (Objeto)lstObjetos.SelectedItem;
                    Objeto objetoNew = Validar();

                    Alterar(objetoOld,objetoNew);
                    Limpar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Alterar(Objeto objetoOld, Objeto objetoNew)
        {
            try
            {
                RemoverDaLista(objetoOld);
                IncluirNaLista(objetoNew);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Deseja salvar os dados?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GravarArquivo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void labelValor_DoubleClick(object sender, EventArgs e)
        {
            txtValor.Visible = !txtValor.Visible;
        }
    }
}
