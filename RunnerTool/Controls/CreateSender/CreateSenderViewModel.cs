using PropertyChanged;
using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class CreateSenderViewModel : BaseVisibleViewModel
	{
		#region privamte members

		Sender curSender;
		string searchText;
		List<BaseInputViewModel> inputVMs;

		#endregion

		#region public properties

		public override bool IsVisible { get; set; } = false;

		//list section
		public List<SenderListItemViewModel> SenderVMs { get; private set; }
		public ObservableCollection<SenderListItemViewModel> FilteredSenderVMs { get; private set; }

		[DoNotCheckEquality]
		public Sender CurSender
		{
			get => curSender;
			set
			{
				curSender = value;
				UpdateInput();
			}
		}

		public string TitleText => (CurSender == null ? "createSender" : "editSender");

		public string OkButtonText => (CurSender == null ? "create" : "finishEdit");

		public bool IsInputSectionVisible { get; private set; }

		[DoNotCheckEquality]
		public string SearchText
		{
			get => searchText;
			set
			{
				searchText = value;
				RecalcFilteredSenders();
			}
		}

		//create/edit section
		public ComboBoxInputViewModel SenderTypeInputVM { get; private set; }
		public StringInputViewModel NameInputVM { get; private set; }
		public StringInputViewModel ShortNameInputVM { get; private set; }

		#endregion

		#region commands

		public ICommand ResetCommand { get; private set; }
		public ICommand OkButtonCommand { get; private set; }
		public ICommand BeginCreationCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CreateSenderViewModel ()
		{
			SenderVMs = new List<SenderListItemViewModel>();
			FilteredSenderVMs = new ObservableCollection<SenderListItemViewModel>();
			UpdateSenderVMs();

			SearchText = "";

			//input fields
			SenderTypeInputVM = new ComboBoxInputViewModel("type",
				DB.Instance.SenderTypes.Select(s => s.Name).ToArray());
			NameInputVM = new StringInputViewModel("name");
			ShortNameInputVM = new StringInputViewModel("shortName");

			inputVMs = new List<BaseInputViewModel>
			{
				SenderTypeInputVM,
				NameInputVM,
				ShortNameInputVM,
			};

			//commands
			ResetCommand = new RelayCommand(ResetCommandMethod);
			OkButtonCommand = new RelayCommand(OkButtonCommandMethod);
			BeginCreationCommand = new RelayCommand(BeginCreationCommandMethod);

			//register events
			DB.Instance.OnSendersChanged += UpdateSenderVMs;
		}

		~CreateSenderViewModel ()
		{
			//unregister events
			DB.Instance.OnSendersChanged -= UpdateSenderVMs;
		}

		#endregion

		#region command methods

		void ResetCommandMethod ()
		{
			ResetInput();
		}

		void OkButtonCommandMethod ()
		{
			//no current destination; create new destination
			if (CurSender == null)
			{
				//create new destination
				Sender newSender = new Sender(
						DB.Instance.SenderTypes[SenderTypeInputVM.SelectedIndex],
						NameInputVM.Text,
						ShortNameInputVM.Text);

				//add to database; callback will update and sort list
				DB.Instance.AddSender(newSender);
			}
			//edit existing destination
			else
			{
				CurSender.Type = DB.Instance.SenderTypes[SenderTypeInputVM.SelectedIndex];
				CurSender.Name = NameInputVM.Text;
				CurSender.ShortName = ShortNameInputVM.Text;

				//force listeners to reload sender list
				DB.Instance.CallOnSendersChanged();
			}

			//changes might affect filtered list; therefore recalc
			RecalcFilteredSenders();

			ResetInput();
		}

		void BeginCreationCommandMethod ()
		{
			BeginCreation();
		}

		#endregion

		#region public methods

		public void ResetInput ()
		{
			CurSender = null;

			//reset input fields
			foreach (var inputVM in inputVMs)
			{
				inputVM.Reset();
			}

			IsInputSectionVisible = false;
		}

		public void UpdateInput ()
		{
			if (CurSender == null)
				return;

			SenderTypeInputVM.SelectedIndex = DB.Instance.SenderTypes.IndexOf(CurSender.Type);
			NameInputVM.Text = CurSender.Name;
			ShortNameInputVM.Text = CurSender.ShortName;

			IsInputSectionVisible = true;
		}

		/// <summary>
		/// Resets and shows the input fields to create a new destination
		/// </summary>
		public void BeginCreation ()
		{
			ResetInput();

			IsInputSectionVisible = true;
		}

		#endregion

		#region private methods

		void UpdateSenderVMs ()
		{
			//retrieve all senders not already contained in sender view models
			DB.Instance.Senders.Except(SenderVMs.Select(s => s.Sender)).ToList().ForEach(s =>
			{
				//add new entries to list
				SenderVMs.Add(new SenderListItemViewModel(s)
				{
					//set callbacks for new entry
					LeftClickCallback = (sender) => CurSender = sender
				});
			});
			SenderVMs.Sort(Comparators.SenderListItemViewModelCompare);

			//recalculate filtered items as well
			RecalcFilteredSenders();
		}

		void RecalcFilteredSenders ()
		{
			//get rid of previous selections
			FilteredSenderVMs.Clear();

			//loop over all senders...
			foreach (var item in SenderVMs)
			{
				//add those, matching search text to list
				if (string.IsNullOrEmpty(SearchText) ||
						item.Sender.Name.ToLower().Contains(SearchText.ToLower()) ||
						item.Sender.ShortName.ToLower().Contains(SearchText.ToLower()))
				{
					FilteredSenderVMs.Add(item);
				}
			}

			//add create new sender button to list
			FilteredSenderVMs.Add(new NewSenderListItemViewModel());
		}

		#endregion
	}
}