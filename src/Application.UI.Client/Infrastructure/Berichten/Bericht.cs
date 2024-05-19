using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdventureArray.Application.UI.Client.Infrastructure.Berichten;

public class Bericht : INotifyPropertyChanged
{
	private Guid _id;
	private string _titel = string.Empty;
	private string _omschrijving = string.Empty;
	private DateTime _tijdstip = DateTime.UtcNow;
	private float? _voortgang;
	private Severity _ernst = Severity.Normal;
	private bool _isAfgerond;

	public Guid Id
	{
		get => _id;
		set => SetField(ref _id, value);
	}

	public string Titel
	{
		get => _titel;
		set => SetField(ref _titel, value);
	}

	public string Details
	{
		get => _omschrijving;
		set => SetField(ref _omschrijving, value);
	}

	public DateTime Tijdstip
	{
		get => _tijdstip;
		set => SetField(ref _tijdstip, value);
	}

	public float? Voortgang
	{
		get => _voortgang;
		set => SetField(ref _voortgang, value);
	}

	public Severity Ernst
	{
		get => _ernst;
		set => SetField(ref _ernst, value);
	}

	public bool IsAfgerond
	{
		get => _isAfgerond;
		set => SetField(ref _isAfgerond, value);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}
}
