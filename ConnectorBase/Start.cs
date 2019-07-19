using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.ConnectorBase
{
    public class Start
    {

        public Start()
        {
            m_runningConnectorId = ConnectorOps.getCurrentConnector();
            FrmConnectorBase frm = new FrmConnectorBase();
            frm.RunningConnectorId = m_runningConnectorId;
            frm.Show();
        }

        int m_runningConnectorId = -1;
        public int RunningConnectorId
        {
            get { return m_runningConnectorId;}
            set { m_runningConnectorId = value; }
        }
    }
}
