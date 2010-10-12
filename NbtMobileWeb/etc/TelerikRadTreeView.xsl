<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="lib.xsl" />

<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:variable name="newline">
<xsl:text>
</xsl:text>
</xsl:variable>


<xsl:template match="NbtTree">
  <xsl:element name="Tree" >
    <xsl:call-template name="Loop">
    </xsl:call-template>
  </xsl:element>
</xsl:template>


<xsl:template name="Loop">
  <xsl:for-each select="NbtNodeGroup">
    <xsl:element name="Node">
      <xsl:attribute name="ID">
        <xsl:value-of select="@key" />
      </xsl:attribute>
      <xsl:attribute name="Value">
        <xsl:value-of select="@key" />
      </xsl:attribute>
      <xsl:attribute name="Text">
        <xsl:value-of select="@name" />
      </xsl:attribute>
      <xsl:attribute name="ImageUrl">
        <xsl:value-of select="@icon" />
      </xsl:attribute>
      <xsl:attribute name="Selectable">
        <xsl:value-of select="@selectable" />
      </xsl:attribute>
      <xsl:attribute name="CssClass">TreeNode</xsl:attribute>
      <xsl:attribute name="HoveredCssClass">HoverTreeNode</xsl:attribute>
      <xsl:attribute name="SelectedCssClass">SelectedTreeNode</xsl:attribute>
      <xsl:attribute name="ExpandMode">
        <xsl:value-of select="@expandmode"/>
      </xsl:attribute>
      <xsl:text>
      </xsl:text>
      <xsl:call-template name="Loop">
      </xsl:call-template>
    </xsl:element>
  </xsl:for-each>

  <xsl:for-each select="NbtNode">
    <xsl:choose>
      <xsl:when test="@showintree='true'">
        <xsl:element name="Node">
          <xsl:attribute name="ID">
            <xsl:value-of select="@key" />
          </xsl:attribute>
          <xsl:attribute name="Value">
            <xsl:value-of select="@key" />
          </xsl:attribute>
          <xsl:attribute name="Text">
            <xsl:value-of select="@nodename" />
          </xsl:attribute>
          <xsl:attribute name="ImageUrl">
            <xsl:value-of select="@iconfilename" />
          </xsl:attribute>
          <xsl:attribute name="Selectable">
            <xsl:value-of select="@selectable" />
          </xsl:attribute>
          <xsl:attribute name="CssClass">TreeNode</xsl:attribute>
          <xsl:attribute name="HoveredCssClass">HoverTreeNode</xsl:attribute>
          <xsl:attribute name="SelectedCssClass">SelectedTreeNode</xsl:attribute>
          <xsl:attribute name="ExpandMode">
            <xsl:value-of select="@expandmode"/>
          </xsl:attribute>
          <xsl:text>
          </xsl:text>
          <xsl:call-template name="Loop">
          </xsl:call-template>
        </xsl:element>
      </xsl:when>

      <xsl:otherwise>
        <xsl:call-template name="Loop">
        </xsl:call-template>
      </xsl:otherwise>
      
    </xsl:choose>
  </xsl:for-each>

</xsl:template>


<xsl:template match="/">
        <xsl:apply-templates/>
</xsl:template>

</xsl:stylesheet>