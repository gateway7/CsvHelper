namespace CsvHelper.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CsvHelper.Configuration;
    using Xunit;

    public class ComplexTest
    {
        public enum AnnotationQuality
        {
            Unknown,

            Low,

            Medium,

            High
        }

        public enum CompoundState
        {
            Unknown,

            Gas,

            Liquid,

            Solid
        }

        public enum CompoundType
        {
            Unknown,

            SmallMoleculeCompound = 1
        }

        [Fact]
        public void CompoundTest()
        {
            using (var textReader = File.OpenText(Path.Combine(Path.GetDirectoryName(typeof(ComplexTest).GetTypeInfo().Assembly.Location), "Resources/compounds.csv")))
            {
                using (var csv = new CsvReader(textReader))
                {
                    csv.Configuration.RegisterClass<Compound>();
                    csv.Configuration.AllowComments = true;
                    csv.Configuration.UnescapeQuotes = true;
                    csv.Configuration.TranslateLiteralNulls = true;
                    csv.Configuration.TypeConverterOptions.Default.TreatNullAsDefault = true;

                    var records = csv.GetRecords<Compound>().ToList();

                    Assert.True(records.Any(p => p.HmdbId > 0));

                    var byState = records.GroupBy(p => p.State);
                    var byType = records.GroupBy(p => p.CompoundType);
                    var byAnnotationQuality = records.GroupBy(p => p.AnnotationQuality);

                    Assert.True(records.Any());
                }
            }
        }

        [Fact]
        public void CompoundLoadTest()
        {
            using (var textReader = File.OpenText(@"d:/downloads/FooDB/compounds.csv"))
            {
                using (var csv = CreateDataReader(textReader))
                {
                    csv.Configuration.RegisterClass<Compound>();

                    var timer = new Stopwatch();
                    timer.Start();
                    var compounds = csv.GetRecords<Compound>().ToList();
                    timer.Stop();
                    Assert.True(timer.Elapsed.TotalSeconds < 15);

                    var haveKingdom = compounds.Where(p => !string.IsNullOrWhiteSpace(p.Kingdom)).ToList();
                    var haveSuperClass = compounds.Where(p => !string.IsNullOrWhiteSpace(p.SuperClass)).ToList();
                    var haveClass = compounds.Where(p => !string.IsNullOrWhiteSpace(p.Class)).ToList();
                    var haveSubClass = compounds.Where(p => !string.IsNullOrWhiteSpace(p.SubClass)).ToList();

                    Assert.Equal(28748, compounds.Count);
                    Assert.Equal(5265, haveKingdom.Count);
                    Assert.Equal(5265, haveSuperClass.Count);
                    Assert.Equal(5225, haveClass.Count);
                    Assert.Equal(4666, haveSubClass.Count);
                }
            }
        }

        public class Compound
        {
            [CsvField("id")]
            public int FoodbId { get; set; }

            public string PublicFoodbId => $"FDB{FoodbId:D6}";

            [CsvField("type")]
            public CompoundType CompoundType { get; set; }

            // [MaxLength(300)]
            [CsvField("name")]
            public string Name { get; set; }

            [CsvField("export")]
            public bool IsExported { get; set; }

            [CsvField("state")]
            public CompoundState State { get; set; }

            [CsvField("annotation_quality")]
            public AnnotationQuality AnnotationQuality { get; set; }

            [CsvField("description")]
            public string Description { get; set; }

            // [MaxLength(130)]
            [CsvField("melting_point")]
            public string MeltingPoint { get; set; }

            [CsvField("created_at")]
            public DateTime CreatedAt { get; set; }

            [CsvField("updated_at")]
            public DateTime UpdatedAt { get; set; }

            // [MaxLength(17)]
            [CsvField("moldb_formula")]
            public string MoldbFormula { get; set; }

            [CsvField("moldb_average_mass")]
            public double MoldbAverageMass { get; set; }

            // [MaxLength(19)]
            [CsvField("kingdom")]
            public string Kingdom { get; set; }

            // [MaxLength(41)]
            [CsvField("superklass")]
            public string SuperClass { get; set; }

            // [MaxLength(49)]
            [CsvField("klass")]
            public string Class { get; set; }

            // [MaxLength(46)]
            [CsvField("subklass")]
            public string SubClass { get; set; }

            // [MaxLength(75)]
            [CsvField("direct_parent")]
            public string DirectParent { get; set; }

            // [MaxLength(36)]
            [CsvField("molecular_framework")]
            public string MolecularFramework { get; set; }

#if false
        [CsvField("legacy_id")]
        public int? LegacyId { get; set; }
#endif

#if false
        [CsvField("protein_formula"), MaxLength(4)]
        public string ProteinFormula { get; set; }

        [CsvField("protein_weight"), MaxLength(4)]
        public string ProteinWeight { get; set; }

        [CsvField("experimental_solubility"), MaxLength(50)]
        public string ExperimentalSolubility { get; set; }

        [CsvField("experimental_logp"), MaxLength(40)]
        public string ExperimentalLogP { get; set; }

        [CsvField("hydrophobicity"), MaxLength(4)]
        public string Hydrophobicity { get; set; }

        [CsvField("isoelectric_point"), MaxLength(4)]
        public string IsoelectricPoint { get; set; }

        [CsvField("metabolism"), MaxLength(474)]
        public string Metabolism { get; set; }
#endif

            // [MaxLength(24)]
            public string CasRegistryNumber { get; set; }

            // [MaxLength(6)]
            [CsvField("kegg_compound_id")]
            public string KeggCompoundId { get; set; }

            [CsvField("pubchem_compound_id")]
            public int PubchemCompoundId { get; set; }

            [CsvField("pubchem_substance_id")]
            public short PubchemSubstanceId { get; set; }

            [CsvField("chebi_id")]
            public int ChebiId { get; set; }

            // [MaxLength(4)]
            [CsvField("het_id")]
            public string HetId { get; set; }

#if false
        [CsvField("uniprot_id")]
        public string UniprotId { get; set; }

        [CsvField("uniprot_name")]
        public string UniprotName { get; set; }

        [CsvField("genbank_id")]
        public string GenbankId { get; set; }
#endif

            // [MaxLength(39)]
            [CsvField("wikipedia_id", Prefix = "http://en.wikipedia.org/wiki/", TrimPrefix = true)]
            public string WikipediaId { get; set; }

#if false
        [CsvField("synthesis_citations")]
        public string SynthesisCitations { get; set; }
#endif

            // [MaxLength(816)]
            [CsvField("general_citations")]
            public string GeneralCitations { get; set; }

            // [MaxLength(682)]
            [CsvField("comments")]
            public string Comments { get; set; }

#if false
        [CsvField("protein_structure_file_name")]
        public string ProteinStructureFileName { get; set; }

        [CsvField("protein_structure_content_type")]
        public string ProteinStructureContentType { get; set; }

        [CsvField("protein_structure_file_size")]
        public int ProteinStructureFileSize { get; set; }

        [CsvField("protein_structure_updated_at")]
        public string ProteinStructureUpdatedAt { get; set; }
#endif

#if false

/// <summary>
/// All data in the floowing block is available through <see cref="HmdbId" />.
/// </summary>
        [CsvField("msds_file_name"), MaxLength(13)]
        public string MsdsFileName { get; set; }

        [CsvField("msds_content_type"), MaxLength(15)]
        public string MsdsContentType { get; set; }

        [CsvField("msds_file_size")]
        public int MsdsFileSize { get; set; }

        [CsvField("msds_updated_at")]
        public DateTime MsdsUpdatedAt { get; set; }
#endif

#if false
        [CsvField("creator_id")]
        public byte CreatorId { get; set; }

        [CsvField("updater_id")]
        public byte UpdaterId { get; set; }
#endif

            [CsvField("phenolexplorer_id")]
            public short PhenolexplorerId { get; set; }

            // [MaxLength(15)]
            [CsvField("dfc_id")]
            public string DfcId { get; set; }

            /// <summary>
            /// Format: HMDB[N5] (e.g. HMDB00205)
            /// URL: http://www.hmdb.ca/metabolites/HMDB[N5]
            /// TODO: convert to type Int16 and extract [N5]
            /// </summary>
            [CsvField("hmdb_id", Prefix = "HMDB", IsPrefixMandatory = true, TrimPrefix = true)]
            public int HmdbId { get; set; }

            // [MaxLength(233)]
            [CsvField("duke_id")]
            public string DukeId { get; set; }

            // [MaxLength(7)]
            [CsvField("drugbank_id")]
            public string DrugBankId { get; set; }

            [CsvField("bigg_id")]
            public int BiggId { get; set; }

            [CsvField("eafus_id")]
            public short EafusId { get; set; }

            // [MaxLength(9)]
            [CsvField("knapsack_id")]
            public string KnapsackId { get; set; }

            // [MaxLength(46)]
            [CsvField("boiling_point")]
            public string BoilingPoint { get; set; }

            // [MaxLength(3)]
            [CsvField("boiling_point_reference")]
            public string BoilingPointReference { get; set; }

            [CsvField("charge")]
            public short Charge { get; set; }

            [CsvField("charge_reference")]
            public string ChargeReference { get; set; }

            // [MaxLength(17)]
            [CsvField("density")]
            public string Density { get; set; }

            // [MaxLength(3)]
            [CsvField("density_reference")]
            public string DensityReference { get; set; }

            // [MaxLength(67)]
            [CsvField("optical_rotation")]
            public string OpticalRotation { get; set; }

            // [MaxLength(3)]
            [CsvField("optical_rotation_reference")]
            public string OpticalRotationReference { get; set; }

            /// <summary>
            /// The percent breakdown of the relative element masses in the molar mass (gram formula mass) of a given molecular
            /// formula.
            /// <see href="https://socratic.org/questions/how-do-you-find-percent-composition-of-a-compound" />
            /// </summary>
            // [MaxLength(66)]
            [CsvField("percent_composition")]
            public string PercentComposition { get; set; }

            /// <summary>
            /// The only values detected in this field so far are [DFC] and [CCD].
            /// CCD possibly refers to Combined Chemical Dictionary (<see href="http://ccd.chemnetbase.com" />).
            /// </summary>
            // [MaxLength(3)]
            [CsvField("percent_composition_reference")]
            public string PercentCompositionReference { get; set; }

            // [MaxLength(46)]
            [CsvField("physical_description")]
            public string PhysicalDescription { get; set; }

            // [MaxLength(3)]
            [CsvField("physical_description_reference")]
            public string PhysicalDescriptionReference { get; set; }

#if false
        [CsvField("refractive_index"), MaxLength(37)]
        public string RefractiveIndex { get; set; }

        [CsvField("refractive_index_reference"), MaxLength(3)]
        public string RefractiveIndexReference { get; set; }

        [CsvField("uv_index"), MaxLength(89)]
        public string UvIndex { get; set; }

        [CsvField("uv_index_reference"), MaxLength(3)]
        public string UvIndexReference { get; set; }

        [CsvField("experimental_pka"), MaxLength(54)]
        public string ExperimentalPka { get; set; }

        [CsvField("experimental_pka_reference"), MaxLength(3)]
        public string ExperimentalPkaReference { get; set; }

        [CsvField("experimental_solubility_reference"), MaxLength(47)]
        public string ExperimentalSolubilityReference { get; set; }

        [CsvField("experimental_logp_reference"), MaxLength(42)]
        public string ExperimentalLogpReference { get; set; }

        [CsvField("hydrophobicity_reference")]
        public string HydrophobicityReference { get; set; }

        [CsvField("isoelectric_point_reference")]
        public string IsoelectricPointReference { get; set; }

        [CsvField("melting_point_reference"), MaxLength(3)]
        public string MeltingPointReference { get; set; }

        [CsvField("moldb_alogps_logp")]
        public double MoldbAlogpsLogp { get; set; }

        [CsvField("moldb_logp")]
        public double MoldbLogp { get; set; }

        [CsvField("moldb_alogps_logs")]
        public double MoldbAlogpsLogs { get; set; }

        [CsvField("moldb_smiles"), MaxLength(1538)]
        public string MoldbSmiles { get; set; }

        [CsvField("moldb_pka")]
        public double MoldbPka { get; set; }
#endif

#if false
        [CsvField("moldb_inchi"), MaxLength(2145)]
        public string MoldbInchi { get; set; }

        [CsvField("moldb_mono_mass")]
        public double MoldbMonoMass { get; set; }

        [CsvField("moldb_inchikey"), MaxLength(36)]
        public string MoldbInchikey { get; set; }

        [CsvField("moldb_alogps_solubility"), MaxLength(12)]
        public string MoldbAlogpsSolubility { get; set; }
#endif

            [CsvField("moldb_id")]
            public int MoldbId { get; set; }

            // [MaxLength(1069)]
            [CsvField("moldb_iupac")]
            public string MoldbIupac { get; set; }

            // [MaxLength(19)]
            [CsvField("structure_source")]
            public string StructureSource { get; set; }

            // [MaxLength(37)]
            [CsvField("duplicate_id")]
            public string DuplicateId { get; set; }

            // [MaxLength(15)]
            [CsvField("old_dfc_id")]
            public string OldDfcId { get; set; }

            // [MaxLength(387)]
            [CsvField("dfc_name")]
            public string DfcName { get; set; }

            // [MaxLength(14)]
            [CsvField("compound_source")]
            public string CompoundSource { get; set; }

            // [MaxLength(11)]
            [CsvField("flavornet_id")]
            public string FlavorNetId { get; set; }

            // [MaxLength(9)]
            [CsvField("goodscent_id")]
            public string GoodScentId { get; set; }

            [CsvField("superscent_id")]
            public int SuperScentId { get; set; }

            [CsvField("phenolexplorer_metabolite_id")]
            public short PhenolExplorerMetaboliteId { get; set; }

            // [MaxLength(13)]
            [CsvField("chembl_id")]
            public string ChemBlId { get; set; }

            [CsvField("chemspider_id")]
            public int ChemSpiderId { get; set; }

            // [MaxLength(40)]
            [CsvField("meta_cyc_id")]
            public string MetaCycId { get; set; }

            [CsvField("foodcomex")]
            public bool FoodComEx { get; set; }

            [CsvField("phytohub_id")]
            public string PhytoHubId { get; set; }
        }

        private static CsvReader CreateDataReader(TextReader textReader)
        {
            var csvReader = new CsvReader(textReader);

            csvReader.Configuration.AllowComments = true;
            csvReader.Configuration.UnescapeQuotes = true;
            csvReader.Configuration.TranslateLiteralNulls = true;
            csvReader.Configuration.TypeConverterOptions.Default.TreatNullAsDefault = true;

            return csvReader;
        }
    }
}