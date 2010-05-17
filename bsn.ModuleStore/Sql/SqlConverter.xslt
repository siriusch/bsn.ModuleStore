<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:t="urn:terminal" xmlns:n="urn:nonterminal" exclude-result-prefixes="msxsl n t">
	<xsl:output method="xml" indent="yes"/>

	<!-- some of the rules can be empty (e.g. "optional"), therefore this key contains all nonterminal elements which do contain any terminals -->
	<xsl:key name="hasTerminal" match="//n:*[descendant::t:*]" use="generate-id()" />

	<xsl:template match="@*|node()" priority="200">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>
	
	<!-- the start rule is StatementList -->
	<xsl:template match="/n:StatementList" priority="100">
		<Sql>
			<xsl:apply-templates />
		</Sql>
	</xsl:template>

	<!-- we replace all terminators and text which is not extracted explicitly (typically whitespace, terminals are processed by another rule) -->
	<xsl:template match="n:Terminator|text()" />

	<!-- in order to keep the structure clear, we match the given block structure in the input -->
	<xsl:template match="n:StatementBlock">
		<Block>
			<xsl:apply-templates/>
		</Block>
	</xsl:template>

	<!-- all statements in a statement list should be terminated with a ";" -->
	<xsl:template match="n:StatementList">
		<xsl:apply-templates>
			<xsl:with-param name="terminate" select="true()"/>
		</xsl:apply-templates>
	</xsl:template>

	<!-- pass-through of the termination information -->
	<xsl:template match="n:StatementGroup">
		<xsl:param name="terminate" select="false()"/>
		<xsl:apply-templates select="*">
			<xsl:with-param name="terminate" select="$terminate"/>
		</xsl:apply-templates>
	</xsl:template>

	<!-- statements are first-class citicens, which declare their type -->
	<xsl:template match="n:Statement">
		<xsl:param name="terminate" select="false()"/>
		<Statement>
			<xsl:attribute name="Kind">
				<xsl:value-of select="substring-before(local-name(n:*/n:*[1]),'Statement')"/>
			</xsl:attribute>
			<xsl:apply-templates />
			<xsl:if test="$terminate">
				<Terminator>;</Terminator>
			</xsl:if>
		</Statement>
	</xsl:template>

	<xsl:template match="n:CTE">
		<CommonTableExpression>
			<xsl:apply-templates />
		</CommonTableExpression>
	</xsl:template>

	<xsl:template match="n:ColumnItem">
		<Column>
			<xsl:apply-templates />
		</Column>
	</xsl:template>

	<xsl:template match="n:Predicate">
		<Predicate>
			<xsl:apply-templates />
		</Predicate>
	</xsl:template>

	<xsl:template match="n:Expression">
		<Expression>
			<xsl:apply-templates />
		</Expression>
	</xsl:template>

	<xsl:template match="n:PredBinaryOp|n:AddBinaryOp|n:MultBinaryOp">
		<Operator>
			<xsl:apply-templates />
		</Operator>
	</xsl:template>

	<xsl:template match="n:StringLiteral">
		<Literal Type="String">
			<xsl:apply-templates>
				<xsl:with-param name="normalize" select="false()" />
			</xsl:apply-templates>
		</Literal>
	</xsl:template>

	<xsl:template match="n:NumberLiteral[not(n:IntegerLiteral)]">
		<Literal Type="Number">
			<xsl:apply-templates/>
		</Literal>
	</xsl:template>

	<xsl:template match="n:IntegerLiteral">
		<Literal Type="Integer">
			<xsl:apply-templates/>
		</Literal>
	</xsl:template>

	<!-- Names -->
	<xsl:template match="n:ColumnNameQualified|n:ColumnWildQualified|n:ProcedureNameQualified|n:TableNameQualified|n:TypeNameQualified">
		<xsl:choose>
			<xsl:when test="(count(*)=1) and (n:*)">
				<xsl:apply-templates />
			</xsl:when>
			<xsl:otherwise>
				<Name>
					<xsl:call-template name="NameType" />
					<xsl:attribute name="Qualified">true</xsl:attribute>
					<xsl:for-each select="n:*">
						<xsl:call-template name="Name" />
					</xsl:for-each>
				</Name>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="n:AliasName|n:CollationName|n:ColumnName|n:ColumnWild|n:ConstraintName|n:FunctionName|n:IndexName|n:LabelName|n:ParameterName|n:ProcedureName|n:TableName|n:TriggerName|n:TypeName|n:SchemaName|n:SystemVariableName|n:VariableName|n:ViewName|n:XmlElementName|n:XmlSchemaCollectionName">
		<Name>
			<xsl:call-template name="NameType" />
			<xsl:call-template name="Name" />
		</Name>
	</xsl:template>

	<xsl:template name="Name">
		<xsl:element name="{local-name()}">
			<xsl:choose>
				<xsl:when test="self::n:TypeName">
					<Id>
						<xsl:apply-templates select="*[1]"/>
					</Id>
					<xsl:apply-templates select="*[position()!=1]"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:element>
	</xsl:template>

	<xsl:template name="NameType">
		<xsl:attribute name="Type">
			<xsl:choose>
				<xsl:when test="contains(local-name(),'Wild')">
					<xsl:text>Wildcard</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="substring-before(local-name(),'Name')"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>

	<!-- Common templates -->

	<!-- all tokens are written to the output -->
	<xsl:template match="t:*">
		<xsl:param name="normalize" select="true()" />
		<xsl:variable name="text">
			<xsl:choose>
				<xsl:when test="$normalize">
					<xsl:value-of select="normalize-space(.)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="."/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:if test="$text">
			<Token>
				<xsl:if test="not($normalize)">
					<xsl:attribute name="xml:space">preserve</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="$text"/>
			</Token>
		</xsl:if>
	</xsl:template>

	<!-- normal operation if to flatten (especially for the list constructs), and only process the elements which do contain terminals -->
	<xsl:template match="n:*">
		<xsl:if test="key('hasTerminal',generate-id())">
			<xsl:apply-templates />
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
