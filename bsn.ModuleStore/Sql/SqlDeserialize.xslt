<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:t="urn:terminal" xmlns:n="urn:nonterminal" exclude-result-prefixes="msxsl n t">
	<xsl:output method="xml" indent="yes"/>
	<xsl:strip-space elements="*"/>

	<!-- some of the rules can be empty (e.g. "optional"), therefore this key contains all nonterminal elements which do contain any terminals -->
	<xsl:key name="hasTerminal" match="//n:*[descendant::t:*]" use="generate-id()" />

	<!-- the start rule is the root -->
	<xsl:template match="/" priority="100">
		<Sql>
			<xsl:apply-templates />
		</Sql>
	</xsl:template>

	<!-- in order to keep the structure clear, we match the given block structure in the input -->
	<xsl:template match="n:StatementBlock">
		<Block>
			<xsl:apply-templates/>
		</Block>
	</xsl:template>

	<xsl:template match="n:CreateTableStatement">
		<Table>
			<TableName>
				<xsl:value-of select="n:TableName"/>
			</TableName>
			<Columns>
				<xsl:apply-templates mode="columns"/>
			</Columns>
			<Constraints>
				<xsl:apply-templates mode="constraints"/>
			</Constraints>
		</Table>
	</xsl:template>

	<xsl:template match="n:ColumnDefinition|n:ColumnComputedDefinition" mode="columns">
		<Column>
			<ColumnName>
				<xsl:value-of select="n:ColumnName"/>
			</ColumnName>
			<xsl:apply-templates select="n:ColumnTypeDefinition|n:Expression">
				<xsl:with-param name="cluster" select="descendant::n:*/n:ConstraintCluster[t:CLUSTERED]" />
			</xsl:apply-templates>
		</Column>
	</xsl:template>

	<xsl:template match="n:Expression">
		<Expression>
			
		</Expression>
	</xsl:template>

	<xsl:template match="n:TypeNameQualified">
		<TypeName>
			<xsl:apply-templates select="n:SchemaName" mode="qualified" />
			<Name>
				<xsl:value-of select="n:TypeName"/>
			</Name>
		</TypeName>
	</xsl:template>

	<xsl:template match="n:NamedColumnConstraint[n:ColumnConstraint]">
		<xsl:param name="cluster" />
		<xsl:apply-templates select="n:ColumnConstraint">
			<xsl:with-param name="name" select="n:NamedConstraint/n:ConstraintName" />
			<xsl:with-param name="cluster" select="$cluster" />
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="n:ConstraintName">
		<ConstraintName>
			<xsl:value-of select="."/>
		</ConstraintName>
	</xsl:template>

	<xsl:template match="n:ColumnConstraint[n:ConstraintPrimaryKey]">
		<xsl:param name="name" />
		<xsl:param name="cluster" />
		<xsl:variable name="keyType">
			<xsl:choose>
				<xsl:when test="n:ConstraintPrimaryKey/t:PRIMARY">PrimaryKey</xsl:when>
				<xsl:when test="n:ConstraintPrimaryKey/t:UNIQUE">Unique</xsl:when>
			</xsl:choose>
		</xsl:variable>
		<xsl:element name="{$keyType}">
			<xsl:apply-templates select="$name" />
			<xsl:apply-templates>
				<xsl:with-param name="clusterDefault" select="boolean(n:ConstraintPrimaryKey/t:PRIMARY_KEY) and not($cluster)" />
			</xsl:apply-templates>
		</xsl:element>
	</xsl:template>

	<xsl:template match="n:ConstraintCluster">
		<xsl:param name="clusterDefault" select="false()" />
		<xsl:if test="t:CLUSTERED or (not(t:*) and $clusterDefault)">
			<Clustered>true</Clustered>
		</xsl:if>
	</xsl:template>
								
	<xsl:template match="n:ConstraintIndex">
		<xsl:choose>
			<xsl:when test="t:WITH_FILLFACTOR">
				<FillFactor>
					<xsl:value-of select="n:IntegerLiteral"/>
				</FillFactor>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="n:IndexOptionGroup[t:WITH]">
		<With>
			<xsl:apply-templates select="n:IndexOptionList" />
		</With>
	</xsl:template>

	<xsl:template match="n:IndexOption">
		<xsl:element name="{t:Id}">
			<xsl:value-of select="n:*"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="n:ConstraintCollate">
		<Collate>
			<xsl:apply-templates />
		</Collate>
	</xsl:template>

	<xsl:template match="n:CollationName">
		<CollationName>
			<xsl:value-of select="."/>
		</CollationName>
	</xsl:template>

	<xsl:template match="n:ConstraintRowguid">
		<RowGuid>true</RowGuid>
	</xsl:template>

	<xsl:template match="n:ConstraintIdentity">
		<Identity>true</Identity>
	</xsl:template>

	<xsl:template match="n:NullCheck[t:NOT]">
		<NotNull>true</NotNull>
	</xsl:template>

	<xsl:template match="n:SchemaName" mode="qualified">
		<Schema>
			<xsl:value-of select="."/>
		</Schema>
	</xsl:template>
								
	<xsl:template match="n:NamedTableConstraint" mode="constraints">
		<Constraint>

		</Constraint>
	</xsl:template>


	<!-- we replace all terminators and text which is not extracted explicitly (typically whitespace, terminals are processed by another rule) -->
	<xsl:template match="n:Terminator|t:*|text()" />
	<xsl:template match="n:Terminator|t:*|text()" mode="columns" />
	<xsl:template match="n:Terminator|t:*|text()" mode="constraints" />

	<!-- normal operation if to flatten (especially for the list constructs), and only process the elements which do contain terminals -->
	<xsl:template match="n:*">
		<xsl:if test="key('hasTerminal',generate-id())">
			<xsl:apply-templates />
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
