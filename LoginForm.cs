#region using문
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YTKLib;
using Microsoft.Win32;
using System.Media;
#endregion
#region introduce
/*
Version: 1.0.1.5 Secret Diary for Programmer
KAKAOTALK NICKNAME:YTK Programmer & Lawyer, NAME: Yim Tae Keun (Korean)
*/
//전체 솔루션 범위에서 diary.txt를 diary.ytk로 전환하였음
#endregion
#region Code region
namespace Project_Jack
{
    public partial class LoginForm : Form
    {
        private Point mousePoint;
        ComputerInformation computer = new ComputerInformation();
        private static int Server = 0;
        private static string Path;
        private int click = 0;
        public string Am = null;

        public LoginForm()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=C:\Program Files\ServerForProjectJack.mdf;Integrated Security = True; Connect Timeout = 30");

        private void Login_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string UserName = computer.UserName;

                RegistryKey keyreader1 = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
                Path = keyreader1.GetValue("DiaryPath").ToString();
                keyreader1.Close();

                if (IDTextbox.TextLength == 0)
                {
                    MessageBox.Show("아이디를 입력해주세요");
                    Cursor = Cursors.Default;
                }
                else
                {
                    if (PasswordTextbox.TextLength == 0)
                    {
                        MessageBox.Show("비밀번호를 입력해주세요");
                        Cursor = Cursors.Default;
                    }
                    else
                    {
                        FileInfo ha = new FileInfo(@"C:\Program Files\ServerForProjectJack.mdf");
                        FileInfo al = new FileInfo(@"C:\Program Files\ServerForProjectJack_log.ldf");
                        if (ha.Exists == false || al.Exists == false || Server == 1)//offline_Login
                        {
                            FileStream checkid = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Open, FileAccess.Read);
                            StreamReader readerid = new StreamReader(checkid);
                            FileStream checkpw = new FileStream(Path + @"\SecretDiary\offline_pw.txt", FileMode.Open, FileAccess.Read);
                            StreamReader readerpw = new StreamReader(checkpw);
                            while (readerid.Peek() > 0 && readerpw.Peek() > 0)
                            {
                                var a = readerid.ReadLine().ToString() == IDTextbox.Text;
                                var b = readerpw.ReadLine().ToString() == PasswordTextbox.Text;
                                if (a && b)//컴퓨터는 한 줄 명령줄에서 두개의 파일을 동시에 읽어들일 수 없으므로 두 파일 탐색 코드를 두개로 분할 시켜 코드를 작성한다.
                                {
                                    readerid.Close();
                                    readerpw.Close();
                                    NotifyIcon.Icon = SystemIcons.Application;
                                    NotifyIcon.BalloonTipTitle = "로그인 성공!!!";
                                    NotifyIcon.BalloonTipText = IDTextbox.Text + "님, SecretDiary에 오신걸 환영합니다.";
                                    NotifyIcon.ShowBalloonTip(1000);

                                    string WhereSave = Path + @"\SecretDiary";
                                    DirectoryInfo dir = new DirectoryInfo(WhereSave + (@"\" + IDTextbox.Text + "_Diary"));

                                    if (dir.Exists == false)
                                    {
                                        dir.Create();
                                    }
                                    this.Hide();

                                    FileStream he = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Create);
                                    StreamWriter sw = new StreamWriter(he);
                                    sw.Write(IDTextbox.Text);
                                    sw.Close();
                                    he.Close();

                                    FileStream fs = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Open, FileAccess.Read);
                                    StreamReader sr = new StreamReader(fs);
                                    string value = sr.ReadLine();
                                    fs.Close();
                                    sr.Close();

                                    FileStream fs2 = new FileStream(Path + @"\SecretDiary\" + value + @"_Diary\ItemsCount.txt", FileMode.Append);
                                    fs2.Close();

                                    Cursor = Cursors.Default;

                                    SecretDiary_Main sdm = new SecretDiary_Main();
                                    sdm.Show();
                                    break;
                                }
                                else if (!(readerid.Peek() > 0 && readerpw.Peek() > 0))
                                {
                                    readerid.Close();
                                    readerpw.Close();
                                    MessageBox.Show("비밀번호 혹은 아이디가 일치하지 않습니다.");
                                    Cursor = Cursors.Default;
                                    break;
                                }
                            }
                        }

                        else//online
                        {
                            SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from SecretDiary where USERNAME='" + IDTextbox.Text + "' and PASSWORD='" + PasswordTextbox.Text + "'", con);
                            DataTable newTable = new DataTable();
                            sda.Fill(newTable);

                            if (newTable.Rows[0][0].ToString() == "1")
                            {
                                NotifyIcon.Icon = SystemIcons.Application;
                                NotifyIcon.BalloonTipTitle = "로그인 성공!!!";
                                NotifyIcon.BalloonTipText = IDTextbox.Text + "님, SecretDiary에 오신걸 환영합니다.";
                                NotifyIcon.ShowBalloonTip(1000);

                                string WhereSave = Path + @"\SecretDiary";
                                DirectoryInfo dir = new DirectoryInfo(WhereSave + (@"\" + IDTextbox.Text + "_Diary"));

                                if (dir.Exists == false)
                                {
                                    dir.Create();
                                }
                                this.Hide();

                                SecretDiary_Main sdm = new SecretDiary_Main();
                                sdm.Show();

                                FileStream he = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Create);
                                StreamWriter sw = new StreamWriter(he);
                                sw.Write(IDTextbox.Text);
                                sw.Close();
                                he.Close();

                                FileStream fs = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Open, FileAccess.Read);
                                StreamReader sr = new StreamReader(fs);
                                string value = sr.ReadLine();
                                fs.Close();
                                sr.Close();

                                FileStream fs2 = new FileStream(Path + @"\SecretDiary\" + value + @"_Diary\ItemsCount.txt", FileMode.Append);
                                fs2.Close();

                                Cursor = Cursors.Default;
                            }
                            else//서버 상 아이디와 패스워드가 없을때
                            {
                                Cursor = Cursors.Default;
                                FileStream checkid = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Open, FileAccess.Read);
                                StreamReader readerid = new StreamReader(checkid);
                                FileStream checkpw = new FileStream(Path + @"\SecretDiary\offline_pw.txt", FileMode.Open, FileAccess.Read);
                                StreamReader readerpw = new StreamReader(checkpw);
                                while (readerid.Peek() > 0 && readerpw.Peek() > 0)
                                {
                                    var a = readerid.ReadLine().ToString() == IDTextbox.Text;
                                    var b = readerpw.ReadLine().ToString() == PasswordTextbox.Text;
                                    if (a && b)
                                    {
                                        readerid.Close();
                                        readerpw.Close();
                                        NotifyIcon.Icon = SystemIcons.Application;
                                        NotifyIcon.BalloonTipTitle = "로그인 성공!!!";
                                        NotifyIcon.BalloonTipText = IDTextbox.Text + "님, SecretDiary에 오신걸 환영합니다.";
                                        NotifyIcon.ShowBalloonTip(1000);
                                        //
                                        SqlCommand scm = new SqlCommand("Select Count(*) from SecretDiary where USERNAME = '" + IDTextbox.Text + "' and PASSWORD = '" + PasswordTextbox.Text + "'", con);
                                        con.Open();
                                        scm.Connection = con;
                                        SqlDataReader sdr = scm.ExecuteReader();
                                        sdr.Close();
                                        bool isExists = (int)scm.ExecuteScalar() > 0;

                                        if (isExists)
                                        {
                                            MessageBox.Show("ERROR. 프로그램을 다시 실행해 주세요.");
                                        }
                                        else
                                        {
                                            String query1 = "INSERT INTO SecretDiary (USERNAME,PASSWORD) VALUES('" + IDTextbox.Text + "', '" + PasswordTextbox.Text + "')";
                                            SqlDataAdapter add1 = new SqlDataAdapter(query1, con);
                                            add1.SelectCommand.ExecuteNonQuery();
                                        }
                                        con.Close();
                                        Cursor = Cursors.Default;
                                        //
                                        string WhereSave = Path + @"\SecretDiary";
                                        DirectoryInfo dir = new DirectoryInfo(WhereSave + (@"\" + IDTextbox.Text + "_Diary"));

                                        if (dir.Exists == false)
                                        {
                                            dir.Create();
                                        }
                                        this.Hide();

                                        FileStream he = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Create);
                                        StreamWriter sw = new StreamWriter(he);
                                        sw.Write(IDTextbox.Text);
                                        sw.Close();
                                        he.Close();

                                        FileStream fs = new FileStream(Path + @"\SecretDiary\log.txt", FileMode.Open, FileAccess.Read);
                                        StreamReader sr = new StreamReader(fs);
                                        string value = sr.ReadLine();
                                        fs.Close();
                                        sr.Close();

                                        FileStream fs2 = new FileStream(Path + @"\SecretDiary\" + value + @"_Diary\ItemsCount.txt", FileMode.Append);
                                        fs2.Close();

                                        Cursor = Cursors.Default;

                                        SecretDiary_Main sdm = new SecretDiary_Main();
                                        sdm.Show();
                                        break;
                                    }
                                    else
                                    {
                                        if (!(readerid.Peek() > 0 && readerpw.Peek() > 0))
                                        {
                                            readerid.Close();
                                            readerpw.Close();
                                            MessageBox.Show("비밀번호 혹은 아이디가 일치하지 않습니다.");
                                            Cursor = Cursors.Default;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(Exception exc)
            {
                MessageBox.Show("ERROR: " + exc.Message + "\n프로그램을 다시 실행해 주세요.");
                Application.Exit();
            }
            
        }

        private void Account_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ComputerInformation computer = new ComputerInformation();
            string UserName = computer.UserName;

            RegistryKey keyreader1 = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
            Path = keyreader1.GetValue("DiaryPath").ToString();
            keyreader1.Close();

            if (IDTextbox.TextLength == 0)
            {
                MessageBox.Show("아이디를 입력해주세요");
                Cursor = Cursors.Default;
            }
            else
            {
                if (PasswordTextbox.TextLength == 0)
                {
                    MessageBox.Show("비밀번호를 입력해주세요");
                    Cursor = Cursors.Default;
                }
                else
                {
                    if(!(PasswordTextbox.Text == PasswordAgain_pwb.Text))
                    {
                        MessageBox.Show("입력하신 비밀번호와 재입력한 비밀번호가 일치하지 않습니다.");
                        Cursor = Cursors.Default;
                    }
                    else
                    {
                        FileInfo ha = new FileInfo(@"C:\Program Files\ServerForProjectJack.mdf");
                        FileInfo al = new FileInfo(@"C:\Program Files\ServerForProjectJack_log.ldf");
                        if (ha.Exists == false || al.Exists == false || Server == 1)//offline_Account
                        {
                            FileStream checkid = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Open, FileAccess.Read);
                            StreamReader readerid = new StreamReader(checkid);
                            while (readerid.Peek() > 0)
                            {
                                if (readerid.ReadLine().ToString() == IDTextbox.Text)//아이디가 회원가입 하려는 것과 겹친다면
                                {
                                    readerid.Close();
                                    MessageBox.Show("이미 있는 아이디입니다.");
                                    Cursor = Cursors.Default;
                                    break;
                                }
                                else//아이디 혹은 패스워드의 중복이 없을때
                                {
                                    if (!(readerid.Peek() > 0))// 다 읽고 나서 아이디의 중복이 없을때
                                    {
                                        readerid.Close();
                                        Cursor = Cursors.Default;
                                        FileStream checkid2 = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Append);
                                        FileStream checkpw2 = new FileStream(Path + @"\SecretDiary\offline_pw.txt", FileMode.Append);
                                        StreamWriter srid = new StreamWriter(checkid2);
                                        StreamWriter srpw = new StreamWriter(checkpw2);
                                        srid.WriteLine(IDTextbox.Text);
                                        srpw.WriteLine(PasswordTextbox.Text);
                                        srid.Close();
                                        srpw.Close();
                                        NotifyIcon.Icon = SystemIcons.Application;
                                        NotifyIcon.BalloonTipTitle = "환영합니다!!!!!";
                                        NotifyIcon.BalloonTipText = "Secret Diary에 가입해주셔서 감사드립니다.";
                                        NotifyIcon.ShowBalloonTip(1000);
                                        break;
                                    }
                                }
                            }
                        }
                        else//online
                        {
                            //SqlCommand scm = new SqlCommand("Select Count(*) from SecretDiary where USERNAME = '" + IDTextbox.Text + "' and PASSWORD = '" + PasswordTextbox.Text + "'", con);
                            SqlCommand scm = new SqlCommand("Select Count(*) from SecretDiary where USERNAME = '" + IDTextbox.Text + "'", con);
                            con.Open();
                            scm.Connection = con;
                            SqlDataReader sdr = scm.ExecuteReader();
                            sdr.Close();
                            bool isExists = (int)scm.ExecuteScalar() > 0;

                            if (isExists)//(int)scm.ExecuteScalar() > 0, 중복이라면
                            {
                                MessageBox.Show("이미 있는 아이디 입니다.");
                            }
                            else
                            {
                                String query1 = "INSERT INTO SecretDiary (USERNAME,PASSWORD) VALUES('" + IDTextbox.Text + "', '" + PasswordTextbox.Text + "')";
                                SqlDataAdapter add1 = new SqlDataAdapter(query1, con);
                                add1.SelectCommand.ExecuteNonQuery();
                                FileStream checkid = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Append);
                                FileStream checkpw = new FileStream(Path + @"\SecretDiary\offline_pw.txt", FileMode.Append);
                                StreamWriter srid = new StreamWriter(checkid);
                                StreamWriter srpw = new StreamWriter(checkpw);
                                srid.WriteLine(IDTextbox.Text);
                                srpw.WriteLine(PasswordTextbox.Text);
                                srid.Close();
                                srpw.Close();
                                NotifyIcon.Icon = SystemIcons.Application;
                                NotifyIcon.BalloonTipTitle = "환영합니다!!!!!";
                                NotifyIcon.BalloonTipText = "Secret Diary에 가입해주셔서 감사드립니다.";
                                NotifyIcon.ShowBalloonTip(1000);
                            }
                            con.Close();
                            Cursor = Cursors.Default;

                            string WhereSave = Path + @"\SecretDiary";
                            DirectoryInfo dir = new DirectoryInfo(WhereSave + (@"\" + IDTextbox.Text + "_Diary"));

                            if (dir.Exists == false)
                            {
                                dir.Create();
                            }
                        }
                    }
                    
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string check;
            Server = 0;
            try
            {
                RegistryKey rkey1 = Registry.CurrentUser.CreateSubKey("Secret Diary").CreateSubKey("Data");
                //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"C# rkey\testsubkey", true);
                check = rkey1.GetValue("DiaryVisit").ToString();
            }catch//방문한적 없을경우
            {
                RegistryKey rkey1 = Registry.CurrentUser.CreateSubKey("Secret Diary").CreateSubKey("Data");
                //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"C# rkey\testsubkey", true);
                rkey1.SetValue("DiaryVisit", "true");

                MessageBox.Show("총칙 16. 컴퓨터프로그램저작물은 특정한 결과를 얻기 위하여 컴퓨터 등 정보처리능력을 가진 장치(이하 컴퓨터라 한다) 내에서 직접 또는 간접으로 사용되는 일련의 지시•명령으로 표현된 창작물을 말한다. 17. 편집물은 저작물이나 부호•문자•음•영상 그 밖의 형태의 자료(이하 소재라 한다)의 집합물을 말하며, 데이터베이스를 포함한다. 18. 편집저작물은 편집물로서 그 소재의 선택•배열 또는 구성에 창작성이 있는 것을 말한다. 19. 데이터베이스는 소재를 체계적으로 배열 또는 구성한 편집물로서 개별적으로 그 소재에 접근하거나 그 소재를 검색할 수 있도록 한 것을 말한다. 20. 데이터베이스제작자는 데이터베이스의 제작 또는 그 소재의 갱신•검증 또는 보충(이하 갱신등이라 한다)에 인적 또는 물적으로 상당한 투자를 한 자를 말한다.   제11조(공표권) ①저작자는 그의 저작물을 공표하거나 공표하지 아니할 것을 결정할 권리를 가진다. ②저작자가 공표되지 아니한 저작물의 저작재산권을 제45조 에 따른 양도, 제46조 에 따른 이용허락, 제57조 에 따른 배타적 발행권의 설정 또는 제63조 에 따른 출판권의 설정을 한 경우에는 그 상대방에게 저작물의 공표를 동의한 것으로 추정한다.   <개정 2009.4.22., 2011.12.2.> ③저작자가 공표되지 아니한 미술저작물•건축저작물 또는 사진저작물(이하 미술저작물등이라 한다)의 원본을 양도한 경우에는 그 상대방에게 저작물의 원본의 전시방식에 의한 공표를 동의한 것으로 추정한다. ④원저작자의 동의를 얻어 작성된 2차적저작물 또는 편집저작물이 공표된 경우에는 그 원저작물도 공표된 것으로 본다. ⑤ 공표하지 아니한 저작물을 저작자가 제31조 의 도서관등에 기증한 경우 별도의 의사를 표시하지 않는 한 기증한 때에 공표에 동의한 것으로 추정한다.   <신설 2011.12.2.>  제12조(성명표시권) ①저작자는 저작물의 원본이나 그 복제물에 또는 저작물의 공표 매체에 그의 실명 또는 이명을 표시할 권리를 가진다. ②저작물을 이용하는 자는 그 저작자의 특별한 의사표시가 없는 때에는 저작자가 그의 실명 또는 이명을 표시한 바에 따라 이를 표시하여야 한다. 다만, 저작물의 성질이나 그 이용의 목적 및 형태 등에 비추어 부득이하다고 인정되는 경우에는 그러하지 아니하다.  제13조(동일성유지권) ①저작자는 그의 저작물의 내용•형식 및 제호의 동일성을 유지할 권리를 가진다. ②저작자는 다음 각 호의 어느 하나에 해당하는 변경에 대하여는 이의(異議)할 수 없다. 다만, 본질적인 내용의 변경은 그러하지 아니하다.   <개정 2009.4.22.>1. 제25조 의 규정에 따라 저작물을 이용하는 경우에 학교교육 목적상 부득이하다고 인정되는 범위 안에서의 표현의 변경2. 건축물의 증축•개축 그 밖의 변형3. 특정한 컴퓨터 외에는 이용할 수 없는 프로그램을 다른 컴퓨터에 이용할 수 있도록 하기 위하여 필요한 범위에서의 변경 4. 프로그램을 특정한 컴퓨터에 보다 효과적으로 이용할 수 있도록 하기 위하여 필요한 범위에서의 변경5. 그 밖에 저작물의 성질이나 그 이용의 목적 및 형태 등에 비추어 부득이하다고 인정되는 범위 안에서의 변경제14조(저작인격권의 일신전속성)①저작인격권은 저작자 일신에 전속한다.②저작자의 사망 후에 그의 저작물을 이용하는 자는 저작자가 생존하였더라면 그 저작인격권의 침해가 될 행위를 하여서는 아니 된다. 다만, 그 행위의 성질 및 정도에 비추어 사회통념상 그 저작자의 명예를 훼손하는 것이 아니라고 인정되는 경우에는 그러하지 아니하다.", "Secret Diary 이용 동의");
                MessageBox.Show("SqlServer 프로그램이 있을시 SqlServer관련 파일인 ServerForProjectJack.mdf 파일과 ServerForProjectJack_log.ldf 파일을 C:\\ProgramFiles에 붙여 주시고 이용하시면 ONLINE 모드로 더욱더 신난 프로그램을 만날 수 있습니다.\n *이 두파일이 없어도 신나게 즐기실 수 있습니다.");
                MessageBox.Show("앞으로 Diary 파일이 저장될 장소를 골라주세요");
                FolderBrowserDialog.ShowDialog();
                RegistryKey rkey = Registry.CurrentUser.CreateSubKey("Secret Diary").CreateSubKey("Data");
                //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"C# rkey\testsubkey", true);
                rkey.SetValue("DiaryPath", FolderBrowserDialog.SelectedPath);

                
            }

            RegistryKey keyreader1 = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
            Path = keyreader1.GetValue("DiaryPath").ToString();
            keyreader1.Close();


            ComputerInformation computer = new ComputerInformation();
            string UserName = computer.UserName;

            FileInfo fs = new FileInfo(@"C:\Program Files\ServerForProjectJack.mdf");
            FileInfo fs2 = new FileInfo(@"C:\Program Files\ServerForProjectJack_log.ldf");
            if (fs.Exists == true && fs2.Exists == true)
            {
                ServerChecker.Text = "ONLINE Mode";
                Server_Change_Button.Visible = true;
            }
            //string WhereSave = @"C:\Users\" + UserName + @"\AppData\Local";
            //MessageBox.Show(Path);
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Path + @"\SecretDiary");

                if (dir.Exists == false)
                {
                    dir.Create();

                }
            }
            catch
            {
                MessageBox.Show("당신이 지정한 폴더가 삭제되었거나 오류로 폴더가 지워졌습니다. 사용자 작업 폴더를 다시 선택해 주세요.", "작업 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FolderBrowserDialog.ShowDialog();
                RegistryKey rkey = Registry.CurrentUser.CreateSubKey("Secret Diary").CreateSubKey("Data");
                //RegistryKey key = Registry.CurrentUser.OpenSubKey(@"C# rkey\testsubkey", true);
                rkey.SetValue("DiaryPath", FolderBrowserDialog.SelectedPath);
            }
            RegistryKey keyreader2 = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
            Path = keyreader2.GetValue("DiaryPath").ToString();
            keyreader2.Close();

            FileInfo cr = new FileInfo(Path + @"\SecretDiary\offline_id.txt");
            if (cr.Exists == false)
            {
                FileStream qwe = new FileStream(Path + @"\SecretDiary\offline_id.txt", FileMode.Create);
                StreamWriter qwer = new StreamWriter(qwe);
                qwer.WriteLine("digitalstudy1603");
                qwer.Close();
            }

            FileInfo cr2 = new FileInfo(Path + @"\SecretDiary\offline_pw.txt");
            if (cr2.Exists == false)
            {
                FileStream qwe = new FileStream(Path + @"\SecretDiary\offline_pw.txt", FileMode.Create);
                StreamWriter qwer = new StreamWriter(qwe);
                qwer.WriteLine("asan1603");
                qwer.Close();
            }

            FileInfo eh = new FileInfo(Path + @"\SecretDiary\offline_log_const");
            if (eh.Exists == false)
            {
                eh.Create();
                //FileStream eha = new FileStream(@"C:\Program Files\SecretDiary\offline_log_const", FileMode.Append, FileAccess.Write);
                //StreamWriter ehar = new StreamWriter(eha);
                //ehar.WriteLine("0");
                //ehar.Close();
            }
            RegistryKey keyreader = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
            Path = keyreader.GetValue("DiaryPath").ToString();
            keyreader.Close();

            Password_Check_Panel.Visible = false;

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
                Am = key.GetValue("Administrator_Mode").ToString();
                key.Close();

                if(Am == "true")
                {
                    Server = 1;
                    ServerChecker.Text = "OFFLINE Mode";
                    Server_Change_Button.Text = "ONLINE 모드 전환";
                    IDTextbox.Text = "digitalstudy1603";
                    PasswordTextbox.Text = "asan1603";
                }
            }
            catch (Exception)
            {

            }

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Secret Diary\Data");
                Am = key.GetValue("Premium").ToString();
                key.Close();

                if (Am == "true")
                {
                    Server = 1;
                    ServerChecker.Text = "OFFLINE Mode";
                    Server_Change_Button.Text = "ONLINE 모드 전환";
                    Premium_Label.Visible = true;
                }
            }
            catch (Exception)
            {

            }

        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//로그인 화면 클릭 버튼
        {
            this.panel2.Location = new System.Drawing.Point(153, 0);
            //PasswordAgain.Visible = false;
            PasswordAgain_pwb.Visible = false;
            Password_Check_Panel.Visible = false;
            Login.Visible = true;
            Account.Visible = false;
            label1.Text = "Secret Diary Login";
            this.AcceptButton = this.Login;
        }

        private void AcountWindow_Click(object sender, EventArgs e)
        {
            this.panel2.Location = new System.Drawing.Point(153, 184);
            //PasswordAgain.Visible = true;
            PasswordAgain_pwb.Visible = true;
            Password_Check_Panel.Visible = true;
            Login.Visible = false;
            Account.Visible = true;
            label1.Text = "Secret Diary Register";
            this.AcceptButton = this.Account;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Mininmize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X),
                this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void Server_Change_Button_Click(object sender, EventArgs e)
        {
            if(Server == 0)
            {
                Server = 1;
                ServerChecker.Text = "OFFLINE Mode";
                MessageBox.Show("OFFLINE 모드로 전환되었습니다.");
                Server_Change_Button.Text = "ONLINE 모드 전환";
            }
            else
            {
                Server = 0;
                ServerChecker.Text = "ONLINE Mode";
                MessageBox.Show("ONLINE 모드로 전환되었습니다.");
                Server_Change_Button.Text = "OFFLINE 모드 전환";
            }
        }

        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            //SaveFileDialog.
            MessageBox.Show(SaveFileDialog.FileName);
            Path = SaveFileDialog.FileName;
        }

        private void FolderBrowserDialog_HelpRequest(object sender, EventArgs e)
        {
            //MessageBox.Show(FolderBrowserDialog.SelectedPath);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if(!(click == -1))
            {
                click++;
                if (click == 5)
                {
                    click = -1;
                    MessageBox.Show("You Found the Easter EGG");
                    this.Hide();
                    EasterEgg easter = new EasterEgg();
                    easter.Show();
                }
            }
        }
    }
}
#endregion
