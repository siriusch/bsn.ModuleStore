<?xml version="1.0" encoding="utf-8"?>
<!--
// bsn ModuleStore database versioning
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:utils="urn:utils" exclude-result-prefixes="msxsl utils">
	<xsl:output method="text" indent="no"/>

	<xsl:param name="engine" />
	<xsl:param name="azure" />
	<xsl:param name="version" />

	<xsl:template match="/">
		<xsl:apply-templates select="*" mode="script" />
	</xsl:template>

	<xsl:template match="Function|Procedure|View|Trigger" mode="script">
		<xsl:value-of select="SQL" />
		<xsl:call-template name="Indexes" />
	</xsl:template>

	<xsl:template match="Type" mode="script">
		<xsl:text xml:space="preserve">CREATE TYPE </xsl:text>
		<xsl:value-of select="@Name"/>
		<xsl:text xml:space="preserve"> FROM </xsl:text>
		<xsl:call-template name="RenderType" />
	</xsl:template>

	<xsl:template match="Table" mode="script">
		<xsl:text xml:space="preserve">CREATE TABLE </xsl:text>
		<xsl:value-of select="@Name"/>
		<xsl:call-template name="RenderTable" />
	</xsl:template>

	<xsl:template match="TableType" mode="script">
		<xsl:text xml:space="preserve">CREATE TYPE </xsl:text>
		<xsl:value-of select="@Name"/>
		<xsl:text xml:space="preserve"> AS TABLE</xsl:text>
		<xsl:call-template name="RenderTable" />
	</xsl:template>

	<xsl:template name="RenderTable">
		<xsl:text xml:space="preserve"> (
	</xsl:text>
		<xsl:for-each select="Column">
			<xsl:value-of select="@Name"/>
			<xsl:choose>
				<!-- computed column -->
				<xsl:when test="@Definition">
					<xsl:text xml:space="preserve"> AS </xsl:text>
					<xsl:value-of select="@Definition"/>
					<xsl:if test="@Persisted = 1">
						<xsl:text xml:space="preserve"> PERSISTED</xsl:text>
						<xsl:if test="@Nullable = 0">
							<xsl:text xml:space="preserve"> NOT NULL</xsl:text>
						</xsl:if>
					</xsl:if>
				</xsl:when>
				<!-- normal column -->
				<xsl:otherwise>
					<xsl:text xml:space="preserve"> </xsl:text>
					<xsl:call-template name="RenderType" />
					<!-- default constraint -->
					<xsl:if test="@Default">
						<xsl:if test="@DefaultName">
							<xsl:text xml:space="preserve"> CONSTRAINT </xsl:text>
							<xsl:value-of select="@DefaultName"/>
						</xsl:if>
						<xsl:text xml:space="preserve"> DEFAULT </xsl:text>
						<xsl:value-of select="@Default"/>
					</xsl:if>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:if test="position()!=last()">
				<xsl:text xml:space="preserve">,
	</xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:for-each select="Index[(@PrimaryKey=1) or (@UniqueConstraint=1)]|ForeignKeyConstraint|CheckConstraint">
			<xsl:sort order="descending" select="count(self::Index[@PrimaryKey=1])" data-type="number"/>
			<xsl:sort order="descending" select="count(self::Index[@UniqueConstraint=1])" data-type="number"/>
			<xsl:sort order="descending" select="count(self::ForeignKeyConstraint)" data-type="number"/>
			<xsl:sort order="ascending" select="@Name" data-type="text"/>
			<xsl:text xml:space="preserve">,
	</xsl:text>
			<xsl:if test="@Name">
				<xsl:text xml:space="preserve">CONSTRAINT </xsl:text>
				<xsl:value-of select="@Name"/>
				<xsl:text xml:space="preserve"> </xsl:text>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="self::Index">
					<xsl:choose>
						<xsl:when test="@PrimaryKey=1">PRIMARY KEY</xsl:when>
						<xsl:when test="@UniqueConstraint=1">UNIQUE</xsl:when>
						<xsl:otherwise>
							<xsl:message terminate="yes">Non-constraint index on table</xsl:message>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text xml:space="preserve"> </xsl:text>
					<xsl:value-of select="@Type"/>
					<xsl:call-template name="IndexColumns">
						<xsl:with-param name="indent" select="'  '" />
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="self::ForeignKeyConstraint">
					<xsl:text>FOREIGN KEY (</xsl:text>
					<xsl:for-each select="Column">
						<xsl:value-of select="@Name" />
						<xsl:if test="position()!=last()">
							<xsl:text xml:space="preserve">, </xsl:text>
						</xsl:if>
					</xsl:for-each>
					<xsl:text xml:space="preserve">) REFERENCES </xsl:text>
					<xsl:value-of select="@Reference" />
					<xsl:text> (</xsl:text>
					<xsl:for-each select="Column">
						<xsl:value-of select="@Reference" />
						<xsl:if test="position()!=last()">
							<xsl:text xml:space="preserve">, </xsl:text>
						</xsl:if>
					</xsl:for-each>
					<xsl:text>)</xsl:text>
					<xsl:if test="@OnDelete">
						<xsl:text xml:space="preserve"> ON DELETE </xsl:text>
						<xsl:value-of select="translate(@OnDelete, '_', ' ')"/>
					</xsl:if>
					<xsl:if test="@OnUpdate">
						<xsl:text xml:space="preserve"> ON UPDATE </xsl:text>
						<xsl:value-of select="translate(@OnUpdate, '_', ' ')"/>
					</xsl:if>
				</xsl:when>
				<xsl:when test="self::CheckConstraint">
					<xsl:text xml:space="preserve">CHECK </xsl:text>
					<xsl:value-of select="@Definition" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:message terminate="yes">Unknown table constraint</xsl:message>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		<xsl:text xml:space="preserve">
)</xsl:text>
		<xsl:call-template name="Indexes" />
	</xsl:template>

	<xsl:template name="RenderType">
		<xsl:value-of select="@Type" />
		<!-- length or precision/scale -->
		<xsl:if test="@Length|@Precision">
			<xsl:text>(</xsl:text>
			<xsl:value-of select="@Length|@Precision"/>
			<xsl:if test="@Scale">
				<xsl:text xml:space="preserve">, </xsl:text>
				<xsl:value-of select="@Scale"/>
			</xsl:if>
			<xsl:text>)</xsl:text>
		</xsl:if>
		<!-- collation -->
		<xsl:if test="@Collation">
			<xsl:text xml:space="preserve"> COLLATE </xsl:text>
			<xsl:value-of select="@Collation"/>
		</xsl:if>
		<!-- rowguidcol -->
		<xsl:if test="@RowGuid = 1">
			<xsl:text xml:space="preserve"> ROWGUIDCOL</xsl:text>
		</xsl:if>
		<!-- identity -->
		<xsl:if test="@IdentitySeed">
			<xsl:text xml:space="preserve"> IDENTITY(</xsl:text>
			<xsl:value-of select="@IdentitySeed"/>
			<xsl:text xml:space="preserve">, </xsl:text>
			<xsl:value-of select="@IdentityIncrement"/>
			<xsl:text>)</xsl:text>
		</xsl:if>
		<!-- nullable -->
		<xsl:if test="@Nullable = 0">
			<xsl:text xml:space="preserve"> NOT</xsl:text>
		</xsl:if>
		<xsl:text xml:space="preserve"> NULL</xsl:text>
	</xsl:template>

	<xsl:template name="Indexes">
		<xsl:for-each select="Index[not((@PrimaryKey=1) or (@UniqueConstraint=1))]">
			<xsl:text xml:space="preserve">;

</xsl:text>
			<xsl:text xml:space="preserve">CREATE </xsl:text>
			<xsl:if test="@Unique">
				<xsl:text xml:space="preserve">UNIQUE </xsl:text>
			</xsl:if>
			<xsl:value-of select="@Type"/>
			<xsl:text xml:space="preserve"> INDEX </xsl:text>
			<xsl:value-of select="@Name"/>
			<xsl:text xml:space="preserve"> ON </xsl:text>
			<xsl:value-of select="../@Name"/>
			<xsl:call-template name="IndexColumns" />
		</xsl:for-each>
	</xsl:template>
	
	<xsl:template name="IndexColumns">
		<xsl:param name="indent" select="''" />
		<xsl:text xml:space="preserve"> (
</xsl:text>
		<xsl:for-each select="Column[not(@IsIncludedColumn=1)]">
			<xsl:value-of select="$indent"/>
			<xsl:text xml:space="preserve">  </xsl:text>
			<xsl:value-of select="@Name" />
			<xsl:if test="@Order">
				<xsl:text xml:space="preserve"> </xsl:text>
				<xsl:value-of select="@Order"/>
			</xsl:if>
			<xsl:if test="position()!=last()">,</xsl:if>
			<xsl:text xml:space="preserve">
</xsl:text>
		</xsl:for-each>
		<xsl:if test="Column[@IsIncludedColumn=1]">
			<xsl:value-of select="$indent"/>
			<xsl:text xml:space="preserve">) INCLUDE (
</xsl:text>
			<xsl:for-each select="Column[@IsIncludedColumn=1]">
				<xsl:value-of select="$indent"/>
				<xsl:text xml:space="preserve">  </xsl:text>
				<xsl:value-of select="@Name" />
				<xsl:if test="position()!=last()">,</xsl:if>
				<xsl:text xml:space="preserve">
</xsl:text>
			</xsl:for-each>
			<xsl:value-of select="$indent"/>
		</xsl:if>
		<xsl:text xml:space="preserve">) </xsl:text>
		<xsl:if test="string-length(@Filter)!=0">
			<xsl:text xml:space="preserve">
</xsl:text>
			<xsl:value-of select="$indent"/>
			<xsl:text xml:space="preserve">WHERE </xsl:text>
			<xsl:value-of select="@Filter"/>
			<xsl:text xml:space="preserve">
</xsl:text>
			<xsl:value-of select="$indent"/>
		</xsl:if>
		<xsl:text xml:space="preserve">WITH </xsl:text>
		<xsl:choose>
			<xsl:when test="@FillFactor">
				<xsl:text xml:space="preserve">FILLFACTOR = </xsl:text>
				<xsl:value-of select="@FillFactor"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text xml:space="preserve">(</xsl:text>
				<xsl:if test="not($azure)">
					<xsl:if test="@Padded=1">
						<xsl:text xml:space="preserve">PAD_INDEX=ON, </xsl:text>
					</xsl:if>
				</xsl:if>
				<xsl:if test="not(ancestor::TableType)">
					<xsl:text xml:space="preserve">STATISTICS_NORECOMPUTE=OFF, </xsl:text>
				</xsl:if>
				<xsl:text xml:space="preserve">IGNORE_DUP_KEY=</xsl:text>
				<xsl:choose>
					<xsl:when test="@IgnoreDuplicateKeys=1">ON</xsl:when>
					<xsl:otherwise>OFF</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="not($azure)">
					<xsl:if test="@AllowRowLocks=0">
						<xsl:text xml:space="preserve">, ALLOW_ROW_LOCKS=OFF</xsl:text>
					</xsl:if>
					<xsl:if test="@AllowPageLocks=0">
						<xsl:text xml:space="preserve">, ALLOW_PAGE_LOCKS=OFF</xsl:text>
					</xsl:if>
				</xsl:if>
				<xsl:text>)</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="XmlSchemaCollection" mode="script">
		<xsl:text xml:space="preserve">CREATE XML SCHEMA COLLECTION </xsl:text>
		<xsl:value-of select="@Name"/>
		<xsl:text xml:space="preserve"> AS N'</xsl:text>
		<xsl:value-of select="utils:AsText(.)" />
		<xsl:text xml:space="preserve">';

</xsl:text>
	</xsl:template>
</xsl:stylesheet>
