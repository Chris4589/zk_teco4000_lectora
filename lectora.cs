using ZKFPEngXControl;

ZKFPEngX zkfpEng = new ZKFPEngX();
private int fpcHandle;
private string m_SN, m_Cur, m_Count;


OnVer();
zkfpEng.OnImageReceived += OnOnImageReceivedZkfpengx2;
zkfpEng.OnCapture += OnOnCaptureZkfpengx2;


OnReg();
zkfpEng.OnImageReceived += OnOnImageReceivedZkfpengx2;
zkfpEng.OnEnroll += OnOnEnrollZkfpengx2;
zkfpEng.OnFeatureInfo += OnOnFeatureInfoZkfpengx2;

bool Finger_Init()
{
    bool Active = false;
    if (zkfpEng.InitEngine() == 0)
    {
        fpcHandle = zkfpEng.CreateFPCacheDB();
        Active = true;
        m_SN = zkfpEng.SensorSN;
        m_Cur = zkfpEng.SensorIndex.ToString();
        m_Count = zkfpEng.SensorCount.ToString();
        MessageBox.Show("Inicio con éxito");
    }
    else
    {
        zkfpEng.EndEngine();
    }
    return Active;
}

void OnReg()
{
    zkfpEng.CancelEnroll();
    zkfpEng.BeginEnroll();

    MessageBox.Show("Comienzo de registro");
}

void OnOnEnrollZkfpengx2(bool ActionResult, object ATemplate)
{
    string sTemp = "";
    if (!ActionResult)
    {
        MessageBox.Show("Registro fallido");
        OnRed();
    }
    else
    {
        MessageBox.Show("registro exitoso");

        sTemp = zkfpEng.GetTemplateAsString();

        try
        {
            //insert 'sTemp' como string

            OnGreen();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    zkfpEng.OnImageReceived -= OnOnImageReceivedZkfpengx2;
    zkfpEng.OnEnroll -= OnOnEnrollZkfpengx2;
    zkfpEng.OnFeatureInfo -= OnOnFeatureInfoZkfpengx2;
    
}
void OnOnImageReceivedZkfpengx2(ref bool AImageDalid)
{

    Graphics g = ptbHuella.CreateGraphics();
    int dc = g.GetHdc().ToInt32();
    zkfpEng.PrintImageAt(dc, 0, 0, zkfpEng.ImageWidth, zkfpEng.ImageHeight);
    object obj = null;
    zkfpEng.GetFingerImage(ref obj);
    byte[] data = (byte[])obj;
    MemoryStream ms = new MemoryStream(data);
    Image image = Image.FromStream(ms);
}
void OnOnFeatureInfoZkfpengx2(int AQuality)
{
    string sTemp = "";

    if (zkfpEng.IsRegister)
        sTemp += "Ponga su dedo: " + zkfpEng.EnrollIndex + " veces para terminar el registro" + "\r\n";

    sTemp += "Calidad de huella digital: ";

    if (AQuality != 0)
    {
        sTemp += "Mala " + AQuality + "\r\n";
        OnRed();
    }
    else
    {
        sTemp += "Buena " + "\r\n";
        OnGreen();
    }
    lblTexto.Text = sTemp;
    lblTexto.ForeColor = Color.Red;
}
void OnVer()
{
    if ( zkfpEng.IsRegister )
        zkfpEng.CancelEnroll();
    
    MessageBox.Show("Verificación");
}
                  
void OnOnCaptureZkfpengx2(bool ActionResult, object ATemplate)
{
    try
    {
        string sTemp, sTemp2;
        bool RegChanged = true;
        int id_fecha;

        sTemp = zkfpEng.GetTemplateAsString();
        if (sql.SQL_Command("select Atemplate, Nombre, apellidos, Carnet, foto, id from Finger", 1))
        {
            while (sql.dr.Read())
            {
                sTemp2 = sql.dr[0].ToString();

                if (zkfpEng.VerFingerFromStr(sTemp, sTemp2, false, RegChanged))
                {
                    //existe la huella crack
                    /*lblNombre.Text = "Nombres: " + sql.dr[1].ToString();
                    lblApellidos.Text = "Apellidos: " + sql.dr[2].ToString();
                    lblCarnet.Text = "Carnet: " + sql.dr[3].ToString();
                    lblFecha.Text = "Entrada: " + lblHora.Text;

                    id_fecha = Convert.ToInt32(sql.dr[5].ToString());

                    if (sql.dr[4] != DBNull.Value)
                    {
                        Image img = ss.Bytes2Image((byte[])sql.dr[4]);

                        if (img != null)
                            this.ptbFoto.Image = img;
                    }*/
                    
                    /*tmClear.Tick += new EventHandler(tmClear_Tick);
                    tmClear.Interval = 5000;
                    tmClear.Start();

                    ss.EmitSound("Concedido.mp3");
                    serialPort1.WriteLine("A");*/
                    OnGreen();
                    break;
                }
            }
        }
        sql.dr.Close();

        zkfpEng.OnImageReceived -= OnOnImageReceivedZkfpengx2;
        zkfpEng.OnCapture -= OnOnCaptureZkfpengx2;
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}
void OnClose()
{
    zkfpEng.EndEngine();

    if (fpcHandle > 0)
        zkfpEng.FreeFPCacheDBEx(fpcHandle);
}
void OnRed()
{
    zkfpEng.ControlSensor(12, 1);
    zkfpEng.ControlSensor(12, 0);
}
void OnGreen()
{
    zkfpEng.ControlSensor(11, 1);
    zkfpEng.ControlSensor(11, 0);
}
void OnBeep()
{
    zkfpEng.ControlSensor(13, 1);
    zkfpEng.ControlSensor(13, 0);
} 