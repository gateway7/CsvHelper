namespace CsvHelper.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using Xunit;
    using System.Reflection;
    using CsvHelper.Configuration;

    public class ComplexTest
    {
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

        public enum CompoundType
        {
            Unknown,

            SmallMoleculeCompound = 1
        }

        public enum CompoundState
        {
            Unknown,

            Gas,

            Liquid,

            Solid
        }

        public enum AnnotationQuality
        {
            Unknown,

            Low,

            Medium,

            High
        }

        public class Compound
        {
            [CsvField("id")]
            public int Id { get; set; }

            [CsvField("legacy_id")]
            public int? LegacyId { get; set; }

            [CsvField("type")]
            public CompoundType CompoundType { get; set; }

            public string PublicId => $"FDB{Id:D6}";

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

            [CsvField("cas_number")]
            public string CasNumber { get; set; }

            [CsvField("melting_point")]
            public string MeltingPoint { get; set; }

#if false
        [CsvField("protein_formula")]
        public string ProteinFormula { get; set; }

        [CsvField("protein_weight")]
        public string ProteinWeight { get; set; }
#endif

            [CsvField("experimental_solubility")]
            public string ExperimentalSolubility { get; set; }

            [CsvField("experimental_logp")]
            public string ExperimentalLogP { get; set; }

#if false
        [CsvField("hydrophobicity")]
        public string Hydrophobicity { get; set; }

        [CsvField("isoelectric_point")]
        public string IsoelectricPoint { get; set; }

        [CsvField("metabolism")]
        public string Metabolism { get; set; }
#endif

            [CsvField("kegg_compound_id")]
            public string KeggCompoundId { get; set; }

            [CsvField("pubchem_compound_id")]
            public int PubchemCompoundId { get; set; }

            [CsvField("pubchem_substance_id")]
            public short PubchemSubstanceId { get; set; }

            [CsvField("chebi_id")]
            public int ChebiId { get; set; }

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

            [CsvField("wikipedia_id")]
            public string WikipediaId { get; set; }

#if false
        [CsvField("synthesis_citations")]
        public string SynthesisCitations { get; set; }
#endif

            [CsvField("general_citations")]
            public string GeneralCitations { get; set; }

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

            /// <summary>
            /// Essentially, all data in the block below is available through <see cref="HmdbId"/>.
            /// </summary>
#if false
        [CsvField("msds_file_name")]
        public string MsdsFileName { get; set; }

        [CsvField("msds_content_type")]
        public string MsdsContentType { get; set; }

        [CsvField("msds_file_size")]
        public int MsdsFileSize { get; set; }

        [CsvField("msds_updated_at")]
        public DateTime MsdsUpdatedAt { get; set; }
#endif

            [CsvField("creator_id")]
            public byte CreatorId { get; set; }

            [CsvField("updater_id")]
            public byte UpdaterId { get; set; }

            [CsvField("created_at")]
            public DateTime CreatedAt { get; set; }

            [CsvField("updated_at")]
            public DateTime UpdatedAt { get; set; }

            [CsvField("phenolexplorer_id")]
            public short PhenolexplorerId { get; set; }

            [CsvField("dfc_id")]
            public string DfcId { get; set; }

            /// <summary>
            /// Format: HMDB[N5] (e.g. HMDB00205)
            /// URL: http://www.hmdb.ca/metabolites/HMDB[N5]
            /// TODO: convert to type Int16 and extract [N5] 
            /// </summary>
            [CsvField("hmdb_id", Prefix = "HMDB", TrimPrefix = true)]
            public short HmdbId { get; set; }

            [CsvField("duke_id")]
            public string DukeId { get; set; }

            [CsvField("drugbank_id")]
            public string DrugbankId { get; set; }

            [CsvField("bigg_id")]
            public int BiggId { get; set; }

            [CsvField("eafus_id")]
            public short EafusId { get; set; }

            [CsvField("knapsack_id")]
            public string KnapsackId { get; set; }

            [CsvField("boiling_point")]
            public string BoilingPoint { get; set; }

            [CsvField("boiling_point_reference")]
            public string BoilingPointReference { get; set; }

            [CsvField("charge")]
            public short Charge { get; set; }

            [CsvField("charge_reference")]
            public string ChargeReference { get; set; }

            [CsvField("density")]
            public string Density { get; set; }

            [CsvField("density_reference")]
            public string DensityReference { get; set; }

            [CsvField("optical_rotation")]
            public string OpticalRotation { get; set; }

            [CsvField("optical_rotation_reference")]
            public string OpticalRotationReference { get; set; }

            [CsvField("percent_composition")]
            public string PercentComposition { get; set; }

            [CsvField("percent_composition_reference")]
            public string PercentCompositionReference { get; set; }

            [CsvField("physical_description")]
            public string PhysicalDescription { get; set; }

            [CsvField("physical_description_reference")]
            public string PhysicalDescriptionReference { get; set; }

            [CsvField("refractive_index")]
            public string RefractiveIndex { get; set; }

            [CsvField("refractive_index_reference")]
            public string RefractiveIndexReference { get; set; }

            [CsvField("uv_index")]
            public string UvIndex { get; set; }

            [CsvField("uv_index_reference")]
            public string UvIndexReference { get; set; }

            [CsvField("experimental_pka")]
            public string ExperimentalPka { get; set; }

            [CsvField("experimental_pka_reference")]
            public string ExperimentalPkaReference { get; set; }

            [CsvField("experimental_solubility_reference")]
            public string ExperimentalSolubilityReference { get; set; }

            [CsvField("experimental_logp_reference")]
            public string ExperimentalLogpReference { get; set; }

            [CsvField("hydrophobicity_reference")]
            public string HydrophobicityReference { get; set; }

            [CsvField("isoelectric_point_reference")]
            public string IsoelectricPointReference { get; set; }

            [CsvField("melting_point_reference")]
            public string MeltingPointReference { get; set; }

            //[CsvField("moldb_alogps_logp")]
            //public double MoldbAlogpsLogp { get; set; }

            //[CsvField("moldb_logp")]
            //public double MoldbLogp { get; set; }

            //[CsvField("moldb_alogps_logs")]
            //public double MoldbAlogpsLogs { get; set; }

            [CsvField("moldb_smiles")]
            public string MoldbSmiles { get; set; }

            [CsvField("moldb_pka")]
            public double MoldbPka { get; set; }

            [CsvField("moldb_formula")]
            public string MoldbFormula { get; set; }

            [CsvField("moldb_average_mass")]
            public double MoldbAverageMass { get; set; }

            [CsvField("moldb_inchi")]
            public string MoldbInchi { get; set; }

            [CsvField("moldb_mono_mass")]
            public double MoldbMonoMass { get; set; }

            [CsvField("moldb_inchikey")]
            public string MoldbInchikey { get; set; }

            [CsvField("moldb_alogps_solubility")]
            public string MoldbAlogpsSolubility { get; set; }

            [CsvField("moldb_id")]
            public int MoldbId { get; set; }

            [CsvField("moldb_iupac")]
            public string MoldbIupac { get; set; }

            [CsvField("structure_source")]
            public string StructureSource { get; set; }

            [CsvField("duplicate_id")]
            public string DuplicateId { get; set; }

            [CsvField("old_dfc_id")]
            public string OldDfcId { get; set; }

            [CsvField("dfc_name")]
            public string DfcName { get; set; }

            [CsvField("compound_source")]
            public string CompoundSource { get; set; }

            [CsvField("flavornet_id")]
            public string FlavornetId { get; set; }

            [CsvField("goodscent_id")]
            public string GoodscentId { get; set; }

            [CsvField("superscent_id")]
            public int SuperscentId { get; set; }

            [CsvField("phenolexplorer_metabolite_id")]
            public short PhenolexplorerMetaboliteId { get; set; }

            [CsvField("kingdom")]
            public string Kingdom { get; set; }

            [CsvField("superklass")]
            public string Superklass { get; set; }

            [CsvField("klass")]
            public string Klass { get; set; }

            [CsvField("subklass")]
            public string Subklass { get; set; }

            [CsvField("direct_parent")]
            public string DirectParent { get; set; }

            [CsvField("molecular_framework")]
            public string MolecularFramework { get; set; }

            [CsvField("chembl_id")]
            public string ChemblId { get; set; }

            [CsvField("chemspider_id")]
            public int ChemspiderId { get; set; }

            [CsvField("meta_cyc_id")]
            public string MetaCycId { get; set; }

            [CsvField("foodcomex")]
            public bool Foodcomex { get; set; }

            [CsvField("phytohub_id")]
            public string PhytohubId { get; set; }
        }
    }
}
