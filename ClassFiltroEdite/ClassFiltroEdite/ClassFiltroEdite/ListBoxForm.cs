using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClassFiltroEdite
{
    // Classe auxiliar para retornar os campos anteriormente digitados numa edição pelo ClassFiltroEdite(editaform)
    public class ListBoxForm : Form
    {
        public ListBox listBox;
        public ListBoxForm()
        {
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            AutoScroll = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  // Nones
            MinimizeBox = false;
            MaximizeBox = false;
            InitializeComponent();
            listBox.TabStop = true; 
        }
        
   
        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void InitializeComponent()
        {
            this.listBox = new ListBox();
            this.SuspendLayout();
            // 
            // ListBoxForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "ListBoxForm";
            this.Load += new System.EventHandler(this.ListBoxForm_Load);
            this.listBox.Dock = DockStyle.Fill;
            this.listBox.KeyDown += ListBox_KeyDown;
            this.Controls.Add(this.listBox);
            this.ResumeLayout(false);

        }

        
        private void ListBoxForm_Load(object sender, EventArgs e)
        {

            listBox.Focus();
            if (listBox.Items.Count > 0) { listBox.SelectedIndex = 0; }
        }
    }
    
    public class ListBoxDialog
    {
        public List<string> Lines { get; set; }
        public int Select { get; set; }
        public string Title { get; set; }
        public ListBoxDialog()
        {
            Lines = new List<string>();
            Title = "";
        }
        public string SelItem()
        {
            string result = "";
            if ((Select > -1) && (Select < Lines.Count))
                result = Lines[Select];
            return result;
        }
       

        public bool Execute()
        {
            bool result = false;
            if (Lines.Count == 0)
                MessageBox.Show("Nenhum Item na Lista");
            ListBoxForm listform = new ListBoxForm();
            try
            {
                listform.listBox.Items.Clear();
                foreach (string linha in Lines)
                    listform.listBox.Items.Add(linha);
                listform.Text = Title;
                if (listform.ShowDialog() == DialogResult.OK)
                {
                    result = true;
                    Select = listform.listBox.SelectedIndex;
                }
            }
            catch { }
            return result;

        }

    
    }

/*
 * TListBoxDialog = class(TComponent)
  private
  FLines : TStrings;
  FSelected : Integer;
  FTitle : string;

  function GetSelItem: string;

//  protected
  //  procedure KeyDown(var key: Word; Shift: TShiftState); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    function Execute: Boolean;
    property SelItem: string read GetSelItem;
    procedure SetLines(Value : TStrings);
    function GetLines : TStrings;

  published
  property Lines:TStrings read getLines write SetLines;
  property Selected: Integer read FSelected write FSelected;
  property Title : string read
   FTitle write FTitle;
  end;


implementation


constructor TListBoxDialog.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FLines := TStringList.Create;
  FTitle := '';
end;

destructor TListBoxDialog.Destroy;
begin
  FLines.Free;
  inherited Destroy;
end;

function TListBoxDialog.GetSelItem: string;
begin
  if selected >= 0 then
     Result := FLines[Selected]
  else
     Result := '';
end;
function TListBoxDialog.GetLines : TStrings;
begin
  Result := FLines;
end;

procedure TListBoxDialog.SetLines(value:TStrings);
begin
  FLines.Assign(Value);
end;

function TListBoxDialog.Execute: Boolean;
var
 ListBoxForm : TListBoxForm;
begin
 if Flines.Count = 0 then
    raise EStringListError.Create('Nenhum Item na Lista');
 ListBoxForm := TlistBoxForm.Create(self);
 try
    ListBoxForm.ListBox1.Items := FLines;
    ListBoxForm.ListBox1.ItemIndex := FSelected;
    ListBoxForm.Caption := FTitle;
    if ListBoxForm.ShowModal = mrOk then
       begin
       Result := True;
       Selected := ListBoxForm.ListBox1.ItemIndex;
       end
    else
       Result := False;

 finally
    ListBoxForm.Destroy;
 end;
end;

 * */



}
