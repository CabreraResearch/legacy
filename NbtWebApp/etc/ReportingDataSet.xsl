<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="lib.xsl" />

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:variable name="newline">
  <xsl:text>
  </xsl:text>
  </xsl:variable>


  <xsl:template match="NbtTree">
    <xsl:element name="Nodes" >
      <xsl:call-template name="Loop">
      </xsl:call-template>
    </xsl:element>
  </xsl:template>


  <xsl:template name="Loop">
    <xsl:param name="ParentPropertyValues" select="''" />

    <xsl:for-each select="NbtNode">      

      <xsl:variable name="PropertyValues">
        <xsl:copy-of select="$ParentPropertyValues" />
        <xsl:call-template name="PropertyLoop">
        </xsl:call-template>
      </xsl:variable>

      <!-- Only output the leaves of the tree -->
      <xsl:if test="count(./NbtNode) = 0">
        <xsl:element name="Node">
          <xsl:copy-of select="$PropertyValues" />
        </xsl:element>
      </xsl:if>

      <xsl:call-template name="Loop">
        <xsl:with-param name="ParentPropertyValues">
          <xsl:copy-of select="$PropertyValues" />
        </xsl:with-param>
      </xsl:call-template>

    </xsl:for-each>

  </xsl:template>

  <xsl:template name="PropertyLoop">
    <xsl:for-each select="NbtNodeProp">
      <xsl:variable name="PropName">
        <xsl:call-template name="makesafe">
          <xsl:with-param name="string" select="@name" />
          <xsl:with-param name="dofirstchar" select="'true'" />
        </xsl:call-template>        
      </xsl:variable>
      <xsl:element name="{$PropName}">
        <xsl:value-of select="@gestalt" />
      </xsl:element>
    </xsl:for-each>
  </xsl:template>

    
  <xsl:template match="/">
          <xsl:apply-templates/>
  </xsl:template>

</xsl:stylesheet>