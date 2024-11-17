--use MedSoft;

--create table Medic(
--	MedicID BIGINT PRIMARY KEY,
--	NumeMedic NVARCHAR(50),
--	PrenumeMedic NVARCHAR(50),
--	Specializare NVARCHAR(50)
--);

--create table Pacient(
--	PacientID BIGINT PRIMARY KEY,
--	CNP NVARCHAR(50),
--	NumePacient NVARCHAR(50),
--	PrenumePacient NVARCHAR(50),
--	Adresa NVARCHAR(50),
--	Asigurare DECIMAL(19, 4)
--);

create table Medicamente(
	MedicamentID BIGINT PRIMARY KEY,
	Denumire NVARCHAR(50),
)

create table Consultatie(
	ConsultatieID BIGINT PRIMARY KEY,
	DataConsultatie DATE,
	Diagnostic NVARCHAR(50),
	Doza NVARCHAR(50),
	PacientID BIGINT,
	MedicamentID BIGINT,
	FOREIGN KEY (PacientID) REFERENCES Pacient(PacientID),
	FOREIGN KEY (MedicamentID) REFERENCES Medicamente(MedicamentID),
);
