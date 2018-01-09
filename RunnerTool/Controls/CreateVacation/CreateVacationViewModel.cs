using PropertyChanged;
using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class CreateVacationViewModel : BaseVisibleViewModel
	{
		#region privamte members

		Vacation curVacation;
		string searchText;
		List<BaseInputViewModel> inputVMs;

		#endregion

		#region public properties

		public override bool IsVisible { get; set; }

		//list section
		public List<VacationListItemViewModel> VacationVMs { get; private set; }
		public ObservableCollection<VacationListItemViewModel> FilteredVacationVMs { get; private set; }

		[DoNotCheckEquality]
		public Vacation CurVacation
		{
			get => curVacation;
			set
			{
				curVacation = value;
				UpdateInput();
			}
		}

		public string TitleText => (CurVacation == null ? "createVacation" : "editVacation");

		public string OkButtonText => (CurVacation == null ? "create" : "finish");

		public bool IsInputSectionVisible { get; private set; }

		[DoNotCheckEquality]
		public string SearchText
		{
			get => searchText;
			set
			{
				searchText = value;
				RecalcFilteredVacations();
			}
		}

		//create/edit section
		public ComboBoxInputViewModel DestinationInputVM { get; private set; }
		public DatePickerInputViewModel BeginDateInputVM { get; private set; }
		public DatePickerInputViewModel EndDateInputVM { get; private set; }

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
		public CreateVacationViewModel ()
		{
			VacationVMs = new List<VacationListItemViewModel>();
			FilteredVacationVMs = new ObservableCollection<VacationListItemViewModel>();
			UpdateVacationVMs();

			SearchText = "";

			//input fields
			DestinationInputVM = new ComboBoxInputViewModel("name", DB.Instance.Destinations.Select(d => d.Name).ToArray());
			BeginDateInputVM = new DatePickerInputViewModel("begin");
			EndDateInputVM = new DatePickerInputViewModel("end");

			inputVMs = new List<BaseInputViewModel>
			{
				DestinationInputVM,
				BeginDateInputVM,
				EndDateInputVM
			};

			//commands
			ResetCommand = new RelayCommand(ResetCommandMethod);
			OkButtonCommand = new RelayCommand(OkButtonCommandMethod);
			BeginCreationCommand = new RelayCommand(BeginCreationCommandMethod);

			//register events
			DB.Instance.OnVacationsChanged += UpdateVacationVMs;
			DB.Instance.OnDestinationsChanged += UpdateDestinationInputVM;
		}

		~CreateVacationViewModel ()
		{
			//unregister events
			DB.Instance.OnVacationsChanged -= UpdateVacationVMs;
			DB.Instance.OnDestinationsChanged -= UpdateDestinationInputVM;
		}

		#endregion

		#region command methods

		void ResetCommandMethod ()
		{
			ResetInput();
		}

		void OkButtonCommandMethod ()
		{
			//no current vacation; create new vacation
			if (CurVacation == null)
			{
				//create new vacation
				Vacation newVacation = new Vacation(
						DB.Instance.Destinations.FirstOrDefault(d => d.Name.Equals(DestinationInputVM.Items[DestinationInputVM.SelectedIndex])),
						BeginDateInputVM.Date, EndDateInputVM.Date);

				//add to database; callback will update and sort list
				DB.Instance.AddVacation(newVacation);
			}
			//edit existing vacation
			else
			{
				CurVacation.SetValues(DB.Instance.Destinations.FirstOrDefault(d => d.Name.Equals(DestinationInputVM.Items[DestinationInputVM.SelectedIndex])),
						BeginDateInputVM.Date, EndDateInputVM.Date);

				//force listeners to reload destination list
				DB.Instance.CallOnVacationsChanged();
			}

			//changes might affect filtered list; therefore recalc
			RecalcFilteredVacations();

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
			CurVacation = null;

			//reset input fields
			foreach (var inputVM in inputVMs)
			{
				inputVM.Reset();
			}

			IsInputSectionVisible = false;
		}

		public void UpdateInput ()
		{
			if (CurVacation == null)
				return;

			DestinationInputVM.SelectedIndex = DB.Instance.Destinations.IndexOf(CurVacation.Destination);
			BeginDateInputVM.Date = CurVacation.BeginDate;
			EndDateInputVM.Date = CurVacation.EndDate;

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

		void UpdateVacationVMs ()
		{
			//retrieve all vacations not already contained in vacation view models
			DB.Instance.Vacations.Except(VacationVMs.Select(v => v.Vacation)).ToList().ForEach(v =>
			{
				//add new entries to list
				VacationVMs.Add(new VacationListItemViewModel(v)
				{
					//set callbacks for new entry
					LeftClickCallback = (vacation) => CurVacation = vacation
				});
			});
			VacationVMs.Sort(Comparators.VacationListItemViewModelCompare);

			//recalculate filtered items as well
			RecalcFilteredVacations();
		}

		void UpdateDestinationInputVM ()
		{
			DestinationInputVM.Items = DB.Instance.Destinations.Select(d => d.Name).ToArray();
		}

		void RecalcFilteredVacations ()
		{
			//get rid of previous selections
			FilteredVacationVMs.Clear();

			//loop over all vacations...
			foreach (var item in VacationVMs)
			{
				//add those to list, whose destination matches search text
				if (string.IsNullOrEmpty(SearchText) ||
						item.Vacation.Destination.Name.ToLower().Contains(SearchText.ToLower()) ||
						item.Vacation.Destination.ShortName.ToLower().Contains(SearchText.ToLower()))
				{
					FilteredVacationVMs.Add(item);
				}
			}

			//add create new vacation button to list
			FilteredVacationVMs.Add(new NewVacationListItemViewModel());
		}

		#endregion
	}
}