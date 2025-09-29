using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiApiContext : DbContext
{
    public SiApiContext()
    {
    }

    public SiApiContext(DbContextOptions<SiApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SiapiApikey> SiapiApikeys { get; set; }

    public virtual DbSet<SiapiLink> SiapiLinks { get; set; }

    public virtual DbSet<SiapiQuerylog> SiapiQuerylogs { get; set; }

    public virtual DbSet<SiapiServizi> SiapiServizis { get; set; }

    public virtual DbSet<SiapiServiziApikey> SiapiServiziApikeys { get; set; }

    public virtual DbSet<SiapiServiziParametri> SiapiServiziParametris { get; set; }

    public virtual DbSet<Testtable> Testtables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("SIAPI")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<SiapiApikey>(entity =>
        {
            entity.HasKey(e => e.IdApikey).HasName("SIAPI_APIKEYS_PK");

            entity.ToTable("SIAPI_APIKEYS");

            entity.Property(e => e.IdApikey)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_APIKEY");
            entity.Property(e => e.Apikey)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("APIKEY");
            entity.Property(e => e.DataFin)
                .HasColumnType("DATE")
                .HasColumnName("DATA_FIN");
            entity.Property(e => e.DataIni)
                .HasColumnType("DATE")
                .HasColumnName("DATA_INI");
            entity.Property(e => e.DescApikey)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DESC_APIKEY");
            entity.Property(e => e.Pwd)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("PWD");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("REFRESH_TOKEN");
            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasColumnType("DATE")
                .HasColumnName("REFRESH_TOKEN_EXPIRY_TIME");
        });

        modelBuilder.Entity<SiapiLink>(entity =>
        {
            entity.HasKey(e => e.IdLink).HasName("SIAPI_LINKS_PK");

            entity.ToTable("SIAPI_LINKS");

            entity.Property(e => e.IdLink)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_LINK");
            entity.Property(e => e.DataFin)
                .HasColumnType("DATE")
                .HasColumnName("DATA_FIN");
            entity.Property(e => e.DataIni)
                .HasColumnType("DATE")
                .HasColumnName("DATA_INI");
            entity.Property(e => e.DescLink)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_LINK");
            entity.Property(e => e.FiltroLink)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FILTRO_LINK");
            entity.Property(e => e.HelpLink)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("HELP_LINK");
            entity.Property(e => e.Link)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("LINK");
            entity.Property(e => e.LinkInterno)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y'")
                .HasColumnName("LINK_INTERNO");
            entity.Property(e => e.NomeLink)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOME_LINK");
        });

        modelBuilder.Entity<SiapiQuerylog>(entity =>
        {
            entity.HasKey(e => e.IdQuerylogs).HasName("SIAPI_QUERYLOGS_PK");

            entity.ToTable("SIAPI_QUERYLOGS");

            entity.Property(e => e.IdQuerylogs)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_QUERYLOGS");
            entity.Property(e => e.IdApikey)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_APIKEY");
            entity.Property(e => e.IdServizio)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO");
            entity.Property(e => e.Json)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("JSON");
            entity.Property(e => e.QueryDate)
                .HasColumnType("DATE")
                .HasColumnName("QUERY_DATE");

            entity.HasOne(d => d.IdApikeyNavigation).WithMany(p => p.SiapiQuerylogs)
                .HasForeignKey(d => d.IdApikey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SIAPI_QUERYLOGS_FK2");

            entity.HasOne(d => d.IdServizioNavigation).WithMany(p => p.SiapiQuerylogs)
                .HasForeignKey(d => d.IdServizio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SIAPI_QUERYLOGS_FK1");
        });

        modelBuilder.Entity<SiapiServizi>(entity =>
        {
            entity.HasKey(e => e.IdServizio).HasName("SIAPI_SERVIZI_PK");

            entity.ToTable("SIAPI_SERVIZI");

            entity.Property(e => e.IdServizio)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO");
            entity.Property(e => e.ApikeyRequired)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y' ")
                .HasColumnName("APIKEY_REQUIRED");
            entity.Property(e => e.AuthRequired)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y'")
                .HasColumnName("AUTH_REQUIRED");
            entity.Property(e => e.DataFin)
                .HasColumnType("DATE")
                .HasColumnName("DATA_FIN");
            entity.Property(e => e.DataIni)
                .HasColumnType("DATE")
                .HasColumnName("DATA_INI");
            entity.Property(e => e.DescServizio)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESC_SERVIZIO");
            entity.Property(e => e.FiltroServizio)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FILTRO_SERVIZIO");
            entity.Property(e => e.HelpServizio)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("HELP_SERVIZIO");
            entity.Property(e => e.NomeServizio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOME_SERVIZIO");
            entity.Property(e => e.SqlStatement)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("SQL_STATEMENT");
        });

        modelBuilder.Entity<SiapiServiziApikey>(entity =>
        {
            entity.HasKey(e => e.IdServizioApikey).HasName("SIAPI_SERVIZI_APIKEYS_PK");

            entity.ToTable("SIAPI_SERVIZI_APIKEYS");

            entity.Property(e => e.IdServizioApikey)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO_APIKEY");
            entity.Property(e => e.DataFin)
                .HasColumnType("DATE")
                .HasColumnName("DATA_FIN");
            entity.Property(e => e.DataIni)
                .HasColumnType("DATE")
                .HasColumnName("DATA_INI");
            entity.Property(e => e.IdApikey)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_APIKEY");
            entity.Property(e => e.IdServizio)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO");

            entity.HasOne(d => d.IdApikeyNavigation).WithMany(p => p.SiapiServiziApikeys)
                .HasForeignKey(d => d.IdApikey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SIAPI_SERVIZI_APIKEYS_FK2");

            entity.HasOne(d => d.IdServizioNavigation).WithMany(p => p.SiapiServiziApikeys)
                .HasForeignKey(d => d.IdServizio)
                .HasConstraintName("SIAPI_SERVIZI_APIKEYS_FK1");
        });

        modelBuilder.Entity<SiapiServiziParametri>(entity =>
        {
            entity.HasKey(e => e.IdServizioParametro).HasName("SIAPI_SERVIZI_PARAMETRI_PK");

            entity.ToTable("SIAPI_SERVIZI_PARAMETRI");

            entity.Property(e => e.IdServizioParametro)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO_PARAMETRO");
            entity.Property(e => e.Datatype)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("'TEXT'")
                .HasColumnName("DATATYPE");
            entity.Property(e => e.FieldAlias)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FIELD_ALIAS");
            entity.Property(e => e.HelpParametro)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("HELP_PARAMETRO");
            entity.Property(e => e.IdServizio)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_SERVIZIO");
            entity.Property(e => e.Mandatory)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'N' ")
                .HasColumnName("MANDATORY");
            entity.Property(e => e.UserField)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USER_FIELD");

            entity.HasOne(d => d.IdServizioNavigation).WithMany(p => p.SiapiServiziParametris)
                .HasForeignKey(d => d.IdServizio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SIAPI_SERVIZI_PARAMETRI_FK1");
        });

        modelBuilder.Entity<Testtable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TESTTABLE");

            entity.Property(e => e.Codice)
                .HasColumnType("NUMBER")
                .HasColumnName("CODICE");
            entity.Property(e => e.Datatest)
                .HasColumnType("DATE")
                .HasColumnName("DATATEST");
            entity.Property(e => e.Descrizione)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DESCRIZIONE");
        });
        modelBuilder.HasSequence("SIAPI_APIKEYS_SEQ");
        modelBuilder.HasSequence("SIAPI_LINKS_SEQ");
        modelBuilder.HasSequence("SIAPI_QUERYLOGS_SEQ");
        modelBuilder.HasSequence("SIAPI_SERVIZI_APIKEYS_SEQ");
        modelBuilder.HasSequence("SIAPI_SERVIZI_PARAMETRI_SEQ");
        modelBuilder.HasSequence("SIAPI_SERVIZI_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
