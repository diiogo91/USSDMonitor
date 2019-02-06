using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using MongoDB.Bson;
using MongoDB.Driver;
using PortalDSEMonitorizacao.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using USSDMonitor.Classes;

namespace USSDMonitor.Frames
{
    public partial class FrmVodacom : Form
    {
        private static String output = "*181#\n";
        private static PortalDSEMonDB conexaoDB = new PortalDSEMonDB();
        private static List<Historico> historicos = new List<Historico>();
        private static List<Tentativa> tentativas = new List<Tentativa>();
        private static List<Contacto> contactos = new List<Contacto>();

        private static Task task = new Task(() => { });
        private bool running = false;
        private int counter = 120;
        private System.Timers.Timer timer;
        private Thread th = null;

        public FrmVodacom()
        {
            InitializeComponent();
            getSerialPortsToComboBox();
        }

        private void getSerialPortsToComboBox()
        {
            foreach (string s in SerialPort.GetPortNames().OrderBy(o => o))
            {
                comboBox1.Items.Add(s);
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = "Antes de iniciar o teste certifique-se de que o Modem GSM (com o SIM inserido) esteja devidamente conectado a uma porta COM! \n" + "Clique o icon 'Play' para Iniciar e/ou 'Stop' para interromper a Monitorização do serviço IZI USSD! \n NB: Os Testes são realizados num intervalo de 2 minutos!";
            this.ControlBox = false;
            UpdateTableContent();
        }

        private void UpdateTableContent()
        {
            DataTable dataTbl = criarTabela();
            if (dataTbl != null)
            {
                dataGridView1.Visible = false;
                dataGridView1.DataSource = dataTbl;
                if (dataGridView1.DataSource != null)
                {
                    dataGridView1.Columns["index"].Visible = false;
                    foreach (DataGridViewRow dgvr in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(dgvr.Cells[3].Value.ToString()))
                            {
                                if (dgvr.Cells[3].Value.ToString().Equals("Teste USSD sem sucesso! Ver Log P/detalhes"))
                                {
                                    dgvr.DefaultCellStyle.BackColor = Color.Red;
                                }
                                else if (dgvr.Cells[3].Value.ToString().Equals("Teste USSD concluido com sucesso!"))
                                {
                                    dgvr.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                    dataGridView1.Refresh();
                    dataGridView1.Visible = true;
                }
            }
        }

        private DataTable criarTabela()
        {
            historicos = conexaoDB.Historicos.Find(new BsonDocument()).ToList();
            List<Historico> SortedLis2t = historicos.OrderByDescending(x => x.Inicio).ThenByDescending(x => x.Inicio.TimeOfDay).ToList();
            historicos = SortedLis2t;
            DataTable tabela = new DataTable();
            tabela.Columns.Add("index", typeof(ObjectId));
            tabela.Columns.Add("Início", typeof(string));
            tabela.Columns.Add("Fim", typeof(string));
            tabela.Columns.Add("Status", typeof(string));
            foreach (Historico historico in historicos)
            {
                if (historico.Operadora == "VODACOM")
                {
                    String inicio = historico.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm"));
                    String fim = historico.Fim.ToString(string.Format("dd/MM/yyyy HH:mm"));
                    tabela.Rows.Add(historico.Id, inicio, fim, historico.Resultado);
                }
            }
            return tabela;
        }

        private void runMonitorizacaoUSSD()
        {
            // Gets executed on a seperate thread and 
            // doesn't block the UI while sleeping 
            textBox2.Text = "Iniciando script....";
            output = "";
            textBox1.Text = output;
            ConnectPhone();
            th = Thread.CurrentThread;
            Historico historico = new Historico();
            Tentativa tentativa = FindTentativa("VODACOM");

            if (GSMConnection.gsmComm.IsOpen() == true && comboBox1.Enabled == false)
            {
                running = true;
                do
                {

                    if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 20)
                    {
                        try
                        {
                            historico = new Historico();
                            historico.Inicio = DateTime.Now;
                            historico.Operadora = "VODACOM";
                            historico.Id = ObjectId.GenerateNewId();
                            tentativa.Inicio = DateTime.Now;
                            if (GSMConnection.gsmComm.IsOpen() == false)
                            {
                                GSMConnection.gsmComm.Open();
                            }
                            tentativa.Nr = tentativa.Nr + 1;
                            tryUSSDCommands(historico, tentativa);
                        }
                        catch (CommException ex)
                        {
                            textBox2.Text = "Servidor USSD retornou mensagem de erro: \n" + ex.Message.ToString();
                            output += "Servidor USSD retornou mensagem de erro: \n" + ex.Message.ToString();
                            Thread.Sleep(5000);
                            updateInfoInTextViews();
                            historico.Fim = DateTime.Now;
                            historico.Success = false;
                            historico.Output = output;
                            historico.Resultado = "Teste USSD sem sucesso! Ver Log P/detalhes";
                            tentativa.Success = false;
                            if (tentativa.Nr == 3)
                            {
                                tentativa.Nr = 0;
                                String mensagem = "Atenção.\nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes.\nHora de Indisponibilidade: " + tentativa.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                                String sms = "Atenção, \nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes. \nCmpts";
                                String subject = "ALERTA - Indisponibilidade Serviço IZI USSD VODACOM (TESTE)";
                                contactos = new List<Contacto>();
                                contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                                contactos.Add(new Contacto("1", "ESSAU GUDO", "843106740"));
                                contactos.Add(new Contacto("1", "DIOGO AMARAL", "841845213"));
                                SendEMAILNOTIFICATION(mensagem, subject);
                                Task obj = new Task(() =>
                                {
                                    SendSMSNOTIFICATION(sms, contactos);
                                });
                                obj.Start();
                                // SendSMSNOTIFICATIONGSM(mensagem, contactos);
                                tentativa.Alertou = true;
                            }

                        }
                        finally
                        {
                            GSMConnection.gsmComm.ReleaseProtocol();
                            GSMConnection.gsmComm.Close();
                            if (historico.Resultado != null)
                            {
                                updateObjectsOnDB(historico, tentativa);
                            }
                            UpdateTableContent();
                            counter = 120;
                            timer = new System.Timers.Timer();
                            timer.Interval = 1000;
                            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                            timer.Start();
                            Thread.Sleep(120000);
                        }
                    }
                    else
                    {
                        textBox2.Text = "Monitorização está configurada para correr das 8h até as 20h...";
                    }

                } while (running == true);
            }
            else
            {
                MessageBox.Show("Falhou conexão com o Modem GSM! \n" + "Pf, tente novamente! Se o erro persisitir reinicie a Aplicação/Sonda/Modem.");
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            counter--;
            if (counter == 0)
            {
                timer.Stop();
                counter = 120;
            }
            else
            {
                textBox2.Text = "Proxima execução do Teste IZI USSD irá iniciar dentro de " + counter + " segundos!";
            }
        }

        private void tryUSSDCommands(Historico historico, Tentativa tentativa)
        {
            //Passo 1 
            GSMConnection.protocol = GSMConnection.gsmComm.GetProtocol();
            string request = "*111#";
            output = "*111#\n ";
            textBox1.Text = output;
            textBox2.Text = "Chamando ... *111#";
            string gottenString = GSMConnection.protocol.ExecAndReceiveMultiple("AT+CUSD=1," + request + ",15");
            string responce = "";
            if (!gottenString.Contains("\r\n+CUSD: 2"))
            {
                do
                {
                    Thread.Sleep(4000);
                    output += gottenString;
                    responce += gottenString;
                } while (GSMConnection.protocol.Receive(out gottenString));

                if (responce != "" && !responce.Contains("down") && !responce.Contains("ERROR"))
                {
                    responce = "";
                    updateInfoInTextViews();
                    //Passo 2
                    request = "8" + char.ConvertFromUtf32(26);
                    textBox2.Text = "Selecionando opção... 8";
                    output += "8\n ";
                    textBox1.Text = output;
                    gottenString = GSMConnection.protocol.ExecAndReceiveMultiple(request);
                    if (!gottenString.Contains("\r\n+CUSD: 2"))
                    {
                        do
                        {
                            Thread.Sleep(4000);
                            output += gottenString;
                            responce += gottenString;
                        } while (GSMConnection.protocol.Receive(out gottenString));

                        if (responce != "" && !responce.Contains("down") && !responce.Contains("ERROR"))
                        {
                            responce = "";
                            updateInfoInTextViews();
                            //Passo 3
                            request = "3" + char.ConvertFromUtf32(26);
                            textBox2.Text = "Selecionando opção... 3";
                            output += "3\n ";
                            textBox1.Text = output;
                            gottenString = GSMConnection.protocol.ExecAndReceiveMultiple(request);
                            if (!gottenString.Contains("\r\n+CUSD: 2"))
                            {
                                do
                                {
                                    Thread.Sleep(4000);
                                    output += gottenString;
                                    responce += gottenString;
                                } while (GSMConnection.protocol.Receive(out gottenString));
                                if (responce != "" && !responce.Contains("down") && !responce.Contains("ERROR"))
                                {
                                    responce = "";
                                    updateInfoInTextViews();
                                    //Passo 4
                                    request = "1" + char.ConvertFromUtf32(26);
                                    textBox2.Text = "Selecionando opção... 1";
                                    output += "1\n ";
                                    textBox1.Text = output;
                                    gottenString = GSMConnection.protocol.ExecAndReceiveMultiple(request);
                                    if (!gottenString.Contains("\r\n+CUSD: 2"))
                                    {
                                        do
                                        {
                                            Thread.Sleep(4000);
                                            output += gottenString;
                                            responce += gottenString;
                                        } while (GSMConnection.protocol.Receive(out gottenString));
                                    }
                                    updateInfoInTextViews();
                                    historico.Fim = DateTime.Now;
                                    historico.Success = true;
                                    textBox2.Text = "Comandos USSD executado com sucesso..";
                                    historico.Resultado = "Teste USSD concluido com sucesso!";
                                    tentativa.Fim = DateTime.Now;
                                    if (tentativa.Success == false && tentativa.Alertou == true)
                                    {
                                        String mensagem = "Anteção: Problema ultrapassado.\nA sonda IZI USSD VODACOM realizou com sucesso o teste.\nHora de Disponibilidade: " + tentativa.Fim.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                                        String sms = "Anteção: Problema ultrapassado, \nO servico IZI USSD VODACOM ja esta disponivel. \nCmpts";
                                        String subject = "ALERTA - Disponibilidade Serviço IZI USSD VODACOM (TESTE)";
                                        contactos = new List<Contacto>();
                                        contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                                        contactos.Add(new Contacto("2", "ESSAU GUDO", "843106740"));
                                        contactos.Add(new Contacto("3", "DIOGO AMARAL", "841845213"));
                                        SendEMAILNOTIFICATION(mensagem, subject);
                                        Task obj = new Task(() =>
                                        {
                                            SendSMSNOTIFICATION(sms, contactos);
                                        });
                                        obj.Start();
                                    }
                                    tentativa.Success = true;
                                    tentativa.Nr = 0;
                                    tentativa.Alertou = false;
                                }
                                else
                                {
                                    output += "Servidor USSD não respondeu ao pedido!\n";
                                    textBox1.Text = output;
                                    historico.Fim = DateTime.Now;
                                    tentativa.Fim = DateTime.Now;
                                    textBox2.Text = "Sem resposta esperada do servidor USSD";
                                    historico.Success = false;
                                    historico.Resultado = "Teste USSD sem sucesso! Ver Log P/detalhes";
                                    tentativa.Success = false;

                                    if (tentativa.Nr == 3)
                                    {
                                        tentativa.Nr = 0;
                                        String mensagem = "Atenção.\nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes.\nHora de Indisponibilidade: " + tentativa.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                                        String sms = "Anteção: Indisponibilidade, \nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes. \nCmpts";
                                        String subject = "ALERTA - Indisponibilidade Serviço IZI USSD VODACOM (TESTE)";
                                        contactos = new List<Contacto>();
                                        contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                                        contactos.Add(new Contacto("2", "ESSAU GUDO", "843106740"));
                                        contactos.Add(new Contacto("3", "DIOGO AMARAL", "841845213"));
                                        SendEMAILNOTIFICATION(mensagem, subject);
                                        Task obj = new Task(() =>
                                        {
                                            SendSMSNOTIFICATION(sms, contactos);
                                        });
                                        obj.Start();
                                        tentativa.Alertou = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            output += "Servidor USSD não respondeu ao pedido!\n";
                            textBox1.Text = output;
                            historico.Fim = DateTime.Now;
                            tentativa.Fim = DateTime.Now;
                            textBox2.Text = "Sem resposta esperada do servidor USSD";
                            historico.Success = false;
                            historico.Resultado = "Teste USSD sem sucesso! Ver Log P/detalhes";
                            tentativa.Success = false;

                            if (tentativa.Nr == 3)
                            {
                                tentativa.Nr = 0;
                                String mensagem = "Atenção.\nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes.\nHora de Indisponibilidade: " + tentativa.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                                String sms = "Anteção: Indisponibilidade, \nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes. \nCmpts";
                                String subject = "ALERTA - Indisponibilidade Serviço IZI USSD VODACOM (TESTE)";
                                contactos = new List<Contacto>();
                                contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                                contactos.Add(new Contacto("2", "ESSAU GUDO", "843106740"));
                                contactos.Add(new Contacto("3", "DIOGO AMARAL", "841845213"));
                                SendEMAILNOTIFICATION(mensagem, subject);
                                Task obj = new Task(() =>
                                {
                                    SendSMSNOTIFICATION(sms, contactos);
                                });
                                obj.Start();
                                tentativa.Alertou = true;
                            }
                        }
                    }
                }
                else
                {
                    output += "Servidor USSD não respondeu ao pedido!\n";
                    textBox1.Text = output;
                    historico.Fim = DateTime.Now;
                    tentativa.Fim = DateTime.Now;
                    textBox2.Text = "Sem resposta esperada do servidor USSD";
                    historico.Success = false;
                    historico.Resultado = "Teste USSD sem sucesso! Ver Log P/detalhes";
                    tentativa.Success = false;
                    if (tentativa.Nr == 3)
                    {
                        tentativa.Nr = 0;
                        String mensagem = "Anteção: Indisponibilidade.\nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes.\nHora de ocorrência: " + tentativa.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                        String sms = "Anteção: Indisponibilidade, \nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes. \nCmpts";
                        String subject = "ALERTA - Indisponibilidade Serviço IZI USSD VODACOM (TESTE)";
                        contactos = new List<Contacto>();
                        contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                        contactos.Add(new Contacto("2", "ESSAU GUDO", "843106740"));
                        contactos.Add(new Contacto("3", "DIOGO AMARAL", "841845213"));
                        SendEMAILNOTIFICATION(mensagem, subject);
                        Task obj = new Task(() =>
                        {
                            SendSMSNOTIFICATION(sms, contactos);
                        });
                        obj.Start();
                        tentativa.Alertou = true;
                    }

                }
            }
            else
            {
                output += "Servidor USSD não respondeu ao pedido!\n";
                textBox1.Text = output;
                historico.Fim = DateTime.Now;
                tentativa.Fim = DateTime.Now;
                textBox2.Text = "Sem resposta esperada do servidor USSD";
                historico.Success = false;
                historico.Resultado = "Teste USSD sem sucesso! Ver Log P/detalhes";
                tentativa.Success = false;
                if (tentativa.Nr == 3)
                {
                    tentativa.Nr = 0;
                    String mensagem = "Anteção: Indisponibilidade.\nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes.\nHora de ocorrência: " + tentativa.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\nNotificação de teste";
                    String sms = "Anteção: Indisponibilidade, \nA sonda IZI USSD VODACOM registou três (3) falhas consecutivas nos testes. \nCmpts";
                    String subject = "ALERTA - Indisponibilidade Serviço IZI USSD VODACOM (TESTE)";
                    contactos = new List<Contacto>();
                    contactos.Add(new Contacto("1", "LUCIA CUMBANE", "844380605"));
                    contactos.Add(new Contacto("2", "ESSAU GUDO", "843106740"));
                    contactos.Add(new Contacto("3", "DIOGO AMARAL", "841845213"));
                    SendEMAILNOTIFICATION(mensagem, subject);
                    Task obj = new Task(() =>
                    {
                        SendSMSNOTIFICATION(sms, contactos);
                    });
                    obj.Start();
                    tentativa.Alertou = true;
                }
            }
        }

        private void SendSMSNOTIFICATION(String mensagem, List<Contacto> contactos)
        {
            foreach (Contacto contacto in contactos)
            {
                if (contacto.Telefone != null)
                {
                    String url = "http://semzseiweb03/WSSMSNotifications/default.aspx?celnbr=" + contacto.Telefone + "&smscontent=" + mensagem;
                    HttpWebRequest request = WebRequest.CreateHttp(url);
                    request.KeepAlive = false;
                    request.Timeout = 5000;
                    request.Proxy = null;
                    request.ServicePoint.ConnectionLeaseTimeout = 5000;
                    request.ServicePoint.MaxIdleTime = 5000;
                    request.ServicePoint.CloseConnectionGroup(request.ConnectionGroupName);
                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            using (var responseStream = new StreamReader(response.GetResponseStream()))
                            {
                                request.Abort();
                                responseStream.Close();
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }




        private static void SendSMSNOTIFICATIONGSM(string mensagem, List<Contacto> contactos)
        {
            List<SmsSubmitPdu> outgoindPdus = new List<SmsSubmitPdu>();
            if (GSMConnection.gsmComm.IsConnected() == true)
            {
                String strSMS = mensagem;
                foreach (Contacto contacto in contactos)
                {
                    if (contacto.Telefone != null)
                    {
                        String cellNO = "+258" + contacto.Telefone;
                        byte dcs = (byte)DataCodingScheme.GeneralCoding.Alpha7BitDefault;
                        SmsSubmitPdu pdu = new SmsSubmitPdu(strSMS, cellNO, dcs);
                        outgoindPdus.Add(pdu);
                    }
                }
                foreach (SmsSubmitPdu pdu in outgoindPdus)
                {
                    GSMConnection.gsmComm.SendMessage(pdu);
                    Thread.Sleep(1000);
                }
            }
        }

        private static void SendEMAILNOTIFICATION(string mensagem, string subject)
        {
            MailMessage mail = new MailMessage("monitorizacao@millenniumbim.co.mz", "monitorizacao@millenniumbim.co.mz");
            mail.From = new MailAddress("monitorizacao@millenniumbim.co.mz");
            mail.Subject = subject;
            string Body = mensagem + "\nOutput:\n" + output;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.TargetName = "semzseisrf01";
            smtp.Host = "semzseisrf01"; //Or Your SMTP Server Address
            smtp.Port = 25;
            smtp.EnableSsl = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(mail);
            mail = null;
            smtp = null;
        }

        private void updateObjectsOnDB(Historico historico, Tentativa tentativa)
        {
            historico.Output = output;
            try
            {
                conexaoDB.Historicos.InsertOne(historico);
                Expression<Func<Tentativa, bool>> filtro = x => x.tentativaID == "VODACOM";
                conexaoDB.Tentativas.ReplaceOne(filtro, tentativa);
            }
            catch (Exception ex)
            {
                textBox2.Text = " Erro de conexão com a Base de dados : \n" + ex.Message.ToString() + "\n" + "verificar se servico MongoDB está a correr!";
            }
        }

        private void updateInfoInTextViews()
        {
            textBox1.Text = output;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ConnectPhone()
        {
            try
            {
                textBox2.Text = "A preparar conexão com o Modem GSM...";
                Thread.Sleep(5000);
                if (GSMConnection.gsmComm.IsConnected() == false)
                {
                    if (GSMConnection.gsmComm.IsOpen() == false)
                    {
                        textBox2.Text = "Tentando conectar-se ao Modem GSM...";
                        Thread.Sleep(5000);
                        GSMConnection.gsmComm.Open();
                        GSMConnection.protocol = GSMConnection.gsmComm.GetProtocol();

                    }
                    else
                    {
                        textBox2.Text = "Conexão já estabelecida com o Modem GSM\n na porta " + GSMConnection.port + "." + " Pronto para executar Script...";
                    }
                }
                else
                {
                    textBox2.Text = "Não foi possivel conectar-se ao Modem GSM. Já existe uma ligação na porta " + GSMConnection.port + ".";
                }
            }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message.ToString();
            }
            finally
            {
                if (GSMConnection.gsmComm.IsConnected() == true)
                {
                    if (GSMConnection.gsmComm.IsOpen() == true)
                    {
                        comboBox1.Enabled = false;
                        textBox2.Text = "Conexão estabelecida ao Modem GSM com sucesso na porta " + GSMConnection.port + ".\n" + " Pronto para executar Script...";
                    }
                }
            }
        }

        private void DisconnectPhone()
        {
            try
            {
                textBox2.Text = "A tentar terminar conexão com o Modem GSM....";
                Thread.Sleep(1000);
                Task obj = new Task(() =>
                {
                    counter = 120;
                    GSMConnection.gsmComm.ReleaseProtocol();
                    GSMConnection.gsmComm.Close();
                });
                obj.Start();
                textBox2.Text = "Conexão com o Modem GSM na porta => " + GSMConnection.port + " terminada com sucesso....";
                getSerialPortsToComboBox();
                comboBox1.Enabled = true;
                running = false;
                textBox1.Text = "";
                output = "";
                textBox2.Text = "Clique para iniciar o teste....";
                UpdateTableContent();
                btnIniciar.BackgroundImage = Properties.Resources.play;
            }
            catch (Exception ex)
            {
                textBox2.Text = "Ocorreu uma falha ao desconectar o Modem GSM: " + ex.Message.ToString() + ". Se o Erro persistir force o termino da Aplicação 'USSDMonitor.exe' pelo Task Manager e faça o restart da Sonda/Modem GSM!!";
                comboBox1.Enabled = false;
                running = true;
            }
        }


        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                this.Invalidate();
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                String id = row.Cells["index"].Value.ToString();
                Historico historico = FindHistorico(id);
                if (historico != null)
                {
                    String details = "Data inicio: " + historico.Inicio.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\n\nData Fim: " + historico.Fim.ToString(string.Format("dd/MM/yyyy HH:mm")) + "\n\nLOG:\n\n" + historico.Output;
                    MessageBox.Show(details);
                }
                else
                {
                    MessageBox.Show("Registro não encontrado! Pf, reinicie a aplicação. Se o erro persistir verifique a disponibilidade da Base de dados --> Serviço MongoDB.exe");
                }
            }
        }

        public Historico FindHistorico(String id)
        {
            Historico hist = new Historico();
            Expression<Func<Historico, bool>> filter = x => x.Id == ObjectId.Parse(id);
            hist = conexaoDB.Historicos.Find(filter).FirstOrDefault();
            return hist;
        }

        public Tentativa FindTentativa(String id)
        {
            Tentativa tent = new Tentativa();
            Expression<Func<Tentativa, bool>> filter = x => x.tentativaID == id;
            tent = conexaoDB.Tentativas.Find(filter).FirstOrDefault();
            return tent;
        }

        private void btnMcel_Click_1(object sender, EventArgs e)
        {
            FrmVodacom.ActiveForm.Close();
            Main frm = new Main();
            frm.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void btnIniciar_Click_1(object sender, EventArgs e)
        {
            btnIniciar.BackgroundImage = Properties.Resources.stop;
            if (running == false)
            {
                GSMConnection.port = comboBox1.SelectedItem.ToString();
                GSMConnection.gsmComm = new GsmCommMain(GSMConnection.port, GSMConnection.baudRate, GSMConnection.timeout);
                task = new Task(runMonitorizacaoUSSD);
                task.Start();
            }
            else if (running == true)
            {
                DialogResult result = MessageBox.Show("Deseja para a monitorização do IZI USSD?", "Parar Monitorização IZI USSD", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Task obj = new Task(DisconnectPhone);
                    obj.Start();
                    th.Interrupt();
                    th.Abort();
                    timer.Stop();
                }
                else
                {
                }
            }
        }
    }
}
