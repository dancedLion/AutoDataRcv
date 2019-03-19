using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmConnDriverEdit : Form
    {
        public FrmConnDriverEdit()
        {
            InitializeComponent();
        }

        AssemblyFile m_file;
        IDriverBase m_driver;
        int m_result = -1;
        ConnDriverBase m_returnedvalue;
        
        public int AddNewConnDriver()
        {
            return EditConnDriver(new ConnDriverSetting
            {
                Id=-1
            });
        }
        public int EditConnDriver(ConnDriverSetting conndriver)
        {
            loadConnDriver(conndriver);
            this.ShowDialog();
            return m_result;
        }
        void initForm()
        {

        }

        public ConnDriverBase ReturnedValue
        {
            get { return m_returnedvalue; }
        }

        void onDriverTypeChanged(Type newDriver)
        {

        }
        
        void loadConnDriver(ConnDriverSetting conndriver)
        {

        }

        void toAddNewRow()
        {

        }
        void toRemoveRow()
        {

        }
        void toSave()
        {

        }

    }
}
